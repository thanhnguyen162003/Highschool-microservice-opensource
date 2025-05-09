using Application.Common.Interfaces.AIInferface;
using Application.Common.Models;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.Net;
using System.Text;
using Application.Common.Models.FlashcardContentModel;
using Newtonsoft.Json;
using UglyToad.PdfPig;
using Polly;
using Polly.Retry;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Application.Services.AIService
{
	public class AIService : IAIService
	{
		private readonly ChatClient _client;
		private readonly AISetting _aiSetting;
		private const int MAX_FLASHCARD_CONTENT_COUNT = 15;
		private readonly ILogger<AIService> _logger;
		private readonly AsyncRetryPolicy _retryPolicy;

		private const string ERROR_API_KEY = "ERROR_AI_SERVICE_INVALID_API_KEY";
		private const string ERROR_RATE_LIMIT = "ERROR_AI_SERVICE_RATE_LIMIT";
		private const string ERROR_TIMEOUT = "ERROR_AI_SERVICE_TIMEOUT";
		private const string ERROR_EMPTY_RESPONSE = "ERROR_AI_SERVICE_EMPTY_RESPONSE";
		private const string ERROR_INVALID_RESPONSE = "ERROR_AI_SERVICE_INVALID_RESPONSE";
		private const string ERROR_GENERAL = "ERROR_AI_SERVICE_GENERAL";
		private const string ERROR_NO_FLASHCARDS = "ERROR_AI_SERVICE_NO_FLASHCARDS";
		private const string ERROR_NO_INPUT = "ERROR_AI_SERVICE_NO_INPUT";

		public AIService(IOptions<AISetting> options, ILogger<AIService> logger)
		{
			_aiSetting = options.Value ?? throw new ArgumentNullException(nameof(options));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			if (string.IsNullOrEmpty(_aiSetting.OpenAIKey))
			{
				_logger.LogCritical("OpenAI API key is missing in configuration");
				throw new InvalidOperationException("OpenAI API key is not configured");
			}

			_client = new ChatClient(model: "gpt-4o", apiKey: _aiSetting.OpenAIKey);

			_retryPolicy = Policy
				.Handle<HttpRequestException>()
				.Or<TimeoutException>()
				.Or<Exception>(ex => ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
				.WaitAndRetryAsync(
					3,
					retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
					onRetry: (exception, timeSpan, retryCount, context) =>
					{
						_logger.LogWarning(exception,
							"Retry {RetryCount} after {RetrySeconds}s delay due to: {ErrorMessage}",
							retryCount, timeSpan.TotalSeconds, exception.Message);
					}
				);
		}
		/*------------------------------------Generate FC OpenAI stream------------------------------------------------*/

		public async IAsyncEnumerable<FlashcardContentCreateRequestModel> GenerateFlashcardContentStreamAsync(
		string? note, IFormFile? fileRaw, string? textRaw, int? numberFlashcard, string? levelHard,
		string? frontTextLong, string? backTextLong, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			var activitySource = new ActivitySource("AIService.GenerateFlashcardContentStream");
			using var activity = activitySource.StartActivity("GenerateFlashcards");
			activity?.AddTag("fileInputSize", fileRaw?.Length ?? 0);
			activity?.AddTag("textRawLength", textRaw?.Length ?? 0);
			activity?.AddTag("noteLength", note?.Length ?? 0);
			activity?.AddTag("requestedFlashcardCount", numberFlashcard ?? MAX_FLASHCARD_CONTENT_COUNT);
			activity?.AddTag("levelHard", levelHard ?? "medium");
			activity?.AddTag("frontTextLong", frontTextLong ?? "short");
			activity?.AddTag("backTextLong", backTextLong ?? "short");
			_logger.LogInformation("Starting GenerateFlashcardContentStreamAsync for {NumberFlashcard} flashcards", numberFlashcard ?? MAX_FLASHCARD_CONTENT_COUNT);

			if (fileRaw == null && string.IsNullOrWhiteSpace(textRaw))
			{
				_logger.LogWarning("No input provided: both file and text are null or empty");
				throw new ArgumentException(ERROR_NO_INPUT);
			}

			string extractedText = note ?? "";

			if (fileRaw != null)
			{
				if (fileRaw.Length > _aiSetting.MaxFileSize)
				{
					_logger.LogWarning("File exceeds maximum allowed size: {Size}MB", fileRaw.Length / (1024 * 1024));
					throw new ArgumentException("File size exceeds the maximum allowed limit.");
				}

				bool isImage = fileRaw.ContentType.StartsWith("image/") ||
							  Path.GetExtension(fileRaw.FileName).ToLower() is ".jpg" or ".png" or ".jpeg";
				bool isPdf = fileRaw.ContentType == "application/pdf" ||
							Path.GetExtension(fileRaw.FileName).ToLower() == ".pdf";

				try
				{
					if (isImage)
					{
						extractedText += "\n" + await ExtractTextFromImage(fileRaw);
					}
					else if (isPdf)
					{
						extractedText += "\n" + await ExtractTextFromPdf(fileRaw);
					}
					else
					{
						string fileText = await ExtractTextFromFile(fileRaw);
						extractedText += "\n" + LimitTextSize(fileText);
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error extracting text from file");
					// Continue with what we have rather than failing completely
				}
			}

			if (!string.IsNullOrWhiteSpace(textRaw))
			{
				extractedText += "\n" + LimitTextSize(textRaw);
			}

			if (string.IsNullOrWhiteSpace(extractedText))
			{
				_logger.LogWarning("No text content extracted from input.");
				throw new ArgumentException("No valid text content found in the provided input.");
			}

			const int maxTokens = 25000;
			List<string> textChunks = SplitTextIntoChunks(extractedText, maxTokens);
			_logger.LogInformation("Content split into {ChunkCount} chunks for processing", textChunks.Count);

			HashSet<string> processedTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			int flashcardCount = 0;
			int targetCount = numberFlashcard ?? MAX_FLASHCARD_CONTENT_COUNT;

			foreach (var chunk in textChunks)
			{
				_logger.LogInformation("Processing text chunk, remaining flashcards: {Remaining}", targetCount - flashcardCount);
				if (flashcardCount >= targetCount || cancellationToken.IsCancellationRequested)
					break;

				IAsyncEnumerable<FlashcardContentCreateRequestModel> chunkFlashcards;
				try
				{
					chunkFlashcards = ProcessTextChunkStreamAsync(
						chunk,
						targetCount - flashcardCount,
						levelHard,
						frontTextLong,
						backTextLong,
						cancellationToken);
				}
				catch (Exception ex) when (!(ex is OperationCanceledException && cancellationToken.IsCancellationRequested))
				{
					_logger.LogError(ex, "Error processing text chunk in streaming mode: {Message}", ex.Message);
					// Continue with next chunk instead of failing completely
					continue;
				}

				await foreach (var flashcard in chunkFlashcards.WithCancellation(cancellationToken))
				{
					if (string.IsNullOrWhiteSpace(flashcard.FlashcardContentTerm) ||
						processedTerms.Contains(flashcard.FlashcardContentTerm))
						continue;

					processedTerms.Add(flashcard.FlashcardContentTerm);
					flashcard.Rank = flashcardCount++;

					yield return flashcard;

					if (flashcardCount >= targetCount || cancellationToken.IsCancellationRequested)
						break;
				}
			}

			if (flashcardCount == 0)
			{
				_logger.LogWarning("No flashcards were generated from input content");
				throw new InvalidOperationException(ERROR_NO_FLASHCARDS);
			}
			_logger.LogInformation("Completed GenerateFlashcardContentStreamAsync, generated {FlashcardCount} flashcards", flashcardCount);
			activity?.SetStatus(ActivityStatusCode.Ok);
		}

		private async IAsyncEnumerable<FlashcardContentCreateRequestModel> ProcessTextChunkStreamAsync(
		string chunk,
		int maxFlashcards,
		string? levelHard = null,
		string? frontTextLong = null,
		string? backTextLong = null,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(chunk))
				yield break;

			string difficulty = levelHard ?? "medium";
			int frontLength = ConvertTextLengthToWordCount(frontTextLong ?? "short");
			int backLength = ConvertTextLengthToWordCount(backTextLong ?? "short");

			var flashcards = new List<FlashcardContentCreateRequestModel>();

			var messages = new List<ChatMessage>
	{
		new SystemChatMessage(
			"You are an AI specialized in creating educational flashcards. Your task is to identify key concepts, terms, " +
			"theories, and definitions from academic and technical content and convert them into effective flashcards. " +
			"Focus on information that is most valuable for learning and recall." +
			"You are using Vietnamese as the primary language, but you can also use English if necessary."),

		new SystemChatMessage(
			$"Extract content with these guidelines:\n" +
			$"1. Generate flashcards one by one\n" +
			$"2. Output each flashcard in JSON format as soon as you create it\n" +
			$"3. Terms should be concise ({Math.Max(1, frontLength - 5)}-{frontLength} words) and represent a single clear concept\n" +
			$"4. Definitions should be comprehensive yet concise ({Math.Max(10, backLength - 10)}-{backLength} words) and provide complete context\n" +
			$"5. Prioritize {difficulty} difficulty concepts that would appear on an exam or be essential for understanding the topic\n" +
			$"6. Include a mix of basic terms and more complex concepts\n" +
			$"7. Create up to {maxFlashcards} flashcards unless the content doesn't contain that many unique concepts\n" +
			$"8. Format each flashcard as: {{\"flashcardContentTerm\": \"Term\", \"flashcardContentDefinition\": \"Definition\"}}\n" +
			$"9. Output each flashcard individually as you create it, not as an array\n" +
			$"10. Output ONLY valid JSON without any explanation text"),

		new UserChatMessage(
			$"Create up to {maxFlashcards} flashcards from the following content. Focus on the most important concepts:\n\n{chunk}")
	};

			using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_aiSetting.AITimeoutSeconds));
			using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
			CancellationToken combinedToken = linkedCts.Token;

			var jsonBuffer = new StringBuilder();
			bool streamCompleted = false;

			try
			{
				var streamResults = new List<FlashcardContentCreateRequestModel>();

				await _retryPolicy.ExecuteAsync(async () =>
				{
					try
					{
						var stream = _client.CompleteChatStreamingAsync(
							messages,
							cancellationToken: combinedToken);

						await foreach (var response in stream.WithCancellation(combinedToken))
						{
							var contentUpdate = response.ContentUpdate.FirstOrDefault()?.Text;

							if (!string.IsNullOrEmpty(contentUpdate))
							{
								jsonBuffer.Append(contentUpdate);

								var (completeJson, remainingJson) = ExtractCompleteJsonObject(jsonBuffer.ToString());

								if (!string.IsNullOrEmpty(completeJson))
								{
									jsonBuffer.Clear();
									jsonBuffer.Append(remainingJson);

									try
									{
										var flashcard = JsonConvert.DeserializeObject<FlashcardContentCreateRequestModel>(completeJson);

										if (flashcard != null &&
											!string.IsNullOrWhiteSpace(flashcard.FlashcardContentTerm) &&
											!string.IsNullOrWhiteSpace(flashcard.FlashcardContentDefinition))
										{
											streamResults.Add(flashcard);
										}
									}
									catch (JsonException ex)
									{
										_logger.LogWarning(ex, "Failed to parse JSON from stream: {Json}", completeJson);
									}
								}
							}
						}

						streamCompleted = true;

						if (jsonBuffer.Length > 0)
						{
							try
							{
								var flashcard = JsonConvert.DeserializeObject<FlashcardContentCreateRequestModel>(jsonBuffer.ToString());

								if (flashcard != null &&
									!string.IsNullOrWhiteSpace(flashcard.FlashcardContentTerm) &&
									!string.IsNullOrWhiteSpace(flashcard.FlashcardContentDefinition))
								{
									streamResults.Add(flashcard);
								}
							}
							catch (JsonException)
							{
								// Ignore parsing errors for possibly incomplete final buffer
							}
						}

						flashcards.AddRange(streamResults);
						return true;
					}
					catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
					{
						_logger.LogWarning("OpenAI streaming request timed out after {Timeout} seconds", _aiSetting.AITimeoutSeconds);
						throw new TimeoutException(ERROR_TIMEOUT);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error in streaming flashcard generation");

						if (ex.Message.Contains("API key", StringComparison.OrdinalIgnoreCase) ||
							ex.Message.Contains("authentication", StringComparison.OrdinalIgnoreCase))
						{
							throw new UnauthorizedAccessException(ERROR_API_KEY);
						}
						if (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
						{
							throw new InvalidOperationException(ERROR_RATE_LIMIT);
						}

						throw; // Re-throw any other exception
					}
				});
			}
			catch (Exception ex) when (!(ex is OperationCanceledException && cancellationToken.IsCancellationRequested))
			{
				_logger.LogError(ex, "Exception in ProcessTextChunkStreamAsync: {Message}", ex.Message);
				throw;
			}

			foreach (var flashcard in flashcards)
			{
				if (cancellationToken.IsCancellationRequested)
					yield break;

				yield return flashcard;
			}
		}

		private (string completeJson, string remainingJson) ExtractCompleteJsonObject(string input)
		{
		if (string.IsNullOrEmpty(input))
			return (string.Empty, string.Empty);

		int openBraceIndex = input.IndexOf('{');
		if (openBraceIndex == -1)
			return (string.Empty, input);

		int depth = 0;
		bool inString = false;
		bool escapeNext = false;

		for (int i = openBraceIndex; i < input.Length; i++)
		{
			char c = input[i];

			if (escapeNext)
			{
				escapeNext = false;
				continue;
			}

			if (c == '\\' && inString)
			{
				escapeNext = true;
				continue;
			}

			if (c == '"')
			{
				inString = !inString;
				continue;
			}

			if (!inString)
			{
				if (c == '{')
					depth++;
				else if (c == '}')
				{
					depth--;

					if (depth == 0)
					{
						// We've found a complete JSON object
						string json = input.Substring(openBraceIndex, i - openBraceIndex + 1);
						string remaining = i + 1 < input.Length ? input.Substring(i + 1) : string.Empty;

						// Validate it's valid JSON before returning
						try
						{
							JsonConvert.DeserializeObject(json);
							return (json, remaining);
						}
						catch
						{
							// Not valid JSON, continue searching
						}
					}
				}
			}
		}

		// No complete JSON object found
		return (string.Empty, input);
	}

		/*------------------------------------Generate FC OpenAI Normal------------------------------------------------*/

		public async Task<ResponseModel> GenerateFlashcardContent(string? note, IFormFile? fileRaw, string? textRaw,
			int? numberFlashcard, string? levelHard, string? frontTextLong, string? backTextLong)
		{
			var activitySource = new ActivitySource("AIService.GenerateFlashcardContent");
			using var activity = activitySource.StartActivity("GenerateFlashcards");
			activity?.AddTag("fileInputSize", fileRaw?.Length ?? 0);
			activity?.AddTag("textRawLength", textRaw?.Length ?? 0);
			activity?.AddTag("noteLength", note?.Length ?? 0);
			activity?.AddTag("requestedFlashcardCount", numberFlashcard ?? MAX_FLASHCARD_CONTENT_COUNT);
			activity?.AddTag("levelHard", levelHard ?? "medium");
			activity?.AddTag("frontTextLong", frontTextLong ?? "short");
			activity?.AddTag("backTextLong", backTextLong ?? "short");

			try
			{
				if (fileRaw == null && string.IsNullOrWhiteSpace(textRaw))
				{
					_logger.LogWarning("No input provided: both file and text are null or empty");
					return new ResponseModel(HttpStatusCode.BadRequest, ERROR_NO_INPUT);
				}

				string extractedText = note ?? "";

				if (fileRaw != null)
				{
					if (fileRaw.Length > _aiSetting.MaxFileSize)
					{
						_logger.LogWarning("File exceeds maximum allowed size: {Size}MB", fileRaw.Length / (1024 * 1024));
						return new ResponseModel(HttpStatusCode.BadRequest, "File size exceeds the maximum allowed limit.");
					}

					bool isImage = fileRaw.ContentType.StartsWith("image/") ||
								   Path.GetExtension(fileRaw.FileName).ToLower() is ".jpg" or ".png" or ".jpeg";
					bool isPdf = fileRaw.ContentType == "application/pdf" ||
								 Path.GetExtension(fileRaw.FileName).ToLower() == ".pdf";

					if (isImage)
					{
						extractedText += "\n" + await ExtractTextFromImage(fileRaw);
					}
					else if (isPdf)
					{
						extractedText += "\n" + await ExtractTextFromPdf(fileRaw);
					}
					else
					{
						string fileText = await ExtractTextFromFile(fileRaw);
						extractedText += "\n" + LimitTextSize(fileText);
					}
				}

				if (!string.IsNullOrWhiteSpace(textRaw))
				{
					extractedText += "\n" + LimitTextSize(textRaw);
				}

				if (string.IsNullOrWhiteSpace(extractedText))
				{
					_logger.LogWarning("No text content extracted from input.");
					return new ResponseModel(HttpStatusCode.BadRequest, "No valid text content found in the provided input.");
				}

				const int maxTokens = 25000;
				List<string> textChunks = SplitTextIntoChunks(extractedText, maxTokens);
				_logger.LogInformation("Content split into {ChunkCount} chunks for processing", textChunks.Count);

				List<FlashcardContentCreateRequestModel> allFlashcards = new List<FlashcardContentCreateRequestModel>();
				List<Exception> chunkErrors = new List<Exception>();

				foreach (var chunk in textChunks)
				{
					try
					{
						var flashcardsFromChunk = await ProcessTextChunk(
							chunk,
							numberFlashcard ?? MAX_FLASHCARD_CONTENT_COUNT,
							levelHard,
							frontTextLong,
							backTextLong);

						if (flashcardsFromChunk.Any())
						{
							var uniqueFlashcards = flashcardsFromChunk.Where(f => !allFlashcards.Any(existing =>
								existing.FlashcardContentTerm?.Equals(f.FlashcardContentTerm, StringComparison.OrdinalIgnoreCase) ?? false))
								.ToList();

							allFlashcards.AddRange(uniqueFlashcards);
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error processing text chunk");
						chunkErrors.Add(ex);
					}
				}

				if (!allFlashcards.Any() && chunkErrors.Any())
				{
					var primaryError = chunkErrors.FirstOrDefault();
					activity?.SetStatus(ActivityStatusCode.Error, primaryError?.Message);

					if (primaryError?.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase) == true)
					{
						return new ResponseModel(HttpStatusCode.TooManyRequests, ERROR_RATE_LIMIT);
					}

					return new ResponseModel(HttpStatusCode.BadRequest, ERROR_NO_FLASHCARDS);
				}

				if (!allFlashcards.Any())
				{
					_logger.LogWarning("No flashcards extracted from any chunk.");
					return new ResponseModel(HttpStatusCode.BadRequest, ERROR_NO_FLASHCARDS);
				}

				for (int i = 0; i < allFlashcards.Count; i++)
				{
					allFlashcards[i].Rank = i;
				}

				if (numberFlashcard.HasValue && allFlashcards.Count > numberFlashcard.Value)
				{
					allFlashcards = allFlashcards.Take(numberFlashcard.Value).ToList();
				}

				_logger.LogInformation("Successfully generated {FlashcardCount} flashcards", allFlashcards.Count);
				activity?.SetStatus(ActivityStatusCode.Ok);

				return new ResponseModel(HttpStatusCode.OK, "Flashcards generated successfully.", allFlashcards);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Unhandled exception in GenerateFlashcardContent");
				activity?.SetStatus(ActivityStatusCode.Error, e.Message);

				if (e.Message.Contains("API key", StringComparison.OrdinalIgnoreCase) ||
					e.Message.Contains("authentication", StringComparison.OrdinalIgnoreCase))
				{
					return new ResponseModel(HttpStatusCode.Unauthorized, ERROR_API_KEY);
				}

				if (e.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
				{
					return new ResponseModel(HttpStatusCode.TooManyRequests, ERROR_RATE_LIMIT);
				}

				if (e is TimeoutException)
				{
					return new ResponseModel(HttpStatusCode.RequestTimeout, ERROR_TIMEOUT);
				}

				return new ResponseModel(HttpStatusCode.InternalServerError, ERROR_GENERAL);
			}
		}

		private async Task<List<FlashcardContentCreateRequestModel>> ProcessTextChunk(
			string chunk,
			int maxFlashcards,
			string? levelHard = null,
			string? frontTextLong = null,
			string? backTextLong = null)
		{
			string difficulty = levelHard ?? "medium";
			int frontLength = ConvertTextLengthToWordCount(frontTextLong ?? "short");
			int backLength = ConvertTextLengthToWordCount(backTextLong ?? "short");

			var chatMessages = new List<ChatMessage>
			{
				new SystemChatMessage(
					"You are an AI specialized in creating educational flashcards. Your task is to identify key concepts, terms, " +
					"theories, and definitions from academic and technical content and convert them into effective flashcards. " +
					"Focus on information that is most valuable for learning and recall." +
					"You are using Vietnamese as the primary language, but you can also use English if necessary."
				),
				new SystemChatMessage(
					$"Extract content with these guidelines:\n" +
					$"1. Terms should be concise ({Math.Max(1, frontLength - 5)}-{frontLength} words) and represent a single clear concept\n" +
					$"2. Definitions should be comprehensive yet concise ({Math.Max(10, backLength - 10)}-{backLength} words) and provide complete context\n" +
					$"3. Prioritize {difficulty} difficulty concepts that would appear on an exam or be essential for understanding the topic\n" +
					$"4. Include a mix of basic terms and more complex concepts\n" +
					$"5. Ensure there are no duplicate terms (case-insensitive)\n" +
					$"6. Return exactly {maxFlashcards} flashcards unless the content doesn't contain that many unique concepts\n" +
					$"7. Format term-definition pairs using proper JSON syntax\n" +
					$"8. Return ONLY valid JSON without any explanation text, formatted as: " +
					$"[{{\"flashcardContentTerm\": \"Term\", \"flashcardContentDefinition\": \"Definition\"}}]"
				),
				new UserChatMessage(
					$"Create {maxFlashcards} flashcards from the following content. Focus on the most important concepts:\n\n{chunk}"
				)
			};

			return await _retryPolicy.ExecuteAsync(async () =>
			{
				using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_aiSetting.AITimeoutSeconds));

				var response = await _client.CompleteChatAsync(chatMessages, cancellationToken: cancellationTokenSource.Token);
				var jsonResponse = response.Value.Content.ElementAtOrDefault(0)?.Text;

				if (string.IsNullOrWhiteSpace(jsonResponse))
				{
					_logger.LogWarning("OpenAI response is empty or null for chunk: {Chunk}", chunk.Substring(0, Math.Min(100, chunk.Length)));
					return new List<FlashcardContentCreateRequestModel>();
				}

				jsonResponse = jsonResponse.Trim();
				jsonResponse = RemoveNonJsonCharacters(jsonResponse);

				try
				{
					var flashcards = JsonConvert.DeserializeObject<List<FlashcardContentCreateRequestModel>>(jsonResponse) ??
									 new List<FlashcardContentCreateRequestModel>();

					flashcards = flashcards.Where(f =>
						!string.IsNullOrWhiteSpace(f.FlashcardContentTerm) &&
						!string.IsNullOrWhiteSpace(f.FlashcardContentDefinition))
						.ToList();

					return flashcards;
				}
				catch (JsonException ex)
				{
					_logger.LogError(ex, "Failed to deserialize OpenAI JSON response: {Response}", jsonResponse);
					throw new InvalidOperationException(ERROR_INVALID_RESPONSE, ex);
				}
			});
		}

		private int ConvertTextLengthToWordCount(string textLength)
		{
			return textLength?.ToLower() switch
			{
				"very short" => 5,
				"short" => 10,
				"medium" => 20,
				"long" => 40,
				"very long" => 60,
				var x when int.TryParse(x, out int wordCount) => wordCount,
				_ => 20
			};
		}

		private async Task<string> ExtractTextFromImage(IFormFile imageFile)
		{
			try
			{
				using var memoryStream = new MemoryStream();
				await imageFile.CopyToAsync(memoryStream);
				byte[] imageBytes = memoryStream.ToArray();

				return await _retryPolicy.ExecuteAsync(async () =>
				{
					var imageMessage = new UserChatMessage(
						new ChatMessageContentPart[]
						{
							ChatMessageContentPart.CreateImagePart(new BinaryData(imageBytes), imageFile.ContentType),
							ChatMessageContentPart.CreateTextPart("Extract all text from this image.")
						}
					);

					using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_aiSetting.AITimeoutSeconds));

					var response = await _client.CompleteChatAsync(new List<ChatMessage>
					{
						new SystemChatMessage("You are an AI that extracts text from images."),
						imageMessage
					}, cancellationToken: cancellationTokenSource.Token);

					var extractedText = response.Value.Content.ElementAtOrDefault(0)?.Text;
					return string.IsNullOrWhiteSpace(extractedText) ? "" : extractedText;
				});
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to extract text from image");
				return "";
			}
		}

		private async Task<string> ExtractTextFromFile(IFormFile file)
		{
			try
			{
				using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
				return await reader.ReadToEndAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error extracting text from file: {FileName}", file.FileName);
				return "";
			}
		}

		private async Task<string> ExtractTextFromPdf(IFormFile pdfFile)
		{
			try
			{
				using var memoryStream = new MemoryStream();
				await pdfFile.CopyToAsync(memoryStream);
				memoryStream.Position = 0;

				using var pdfDocument = PdfDocument.Open(memoryStream);
				StringBuilder text = new StringBuilder();

				foreach (var page in pdfDocument.GetPages())
				{
					text.AppendLine(page.Text);
				}

				return text.ToString().Trim();
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to extract text from PDF: {ErrorType}", e.GetType().Name);
				return "";
			}
		}
		private string LimitTextSize(string text)
		{
			const int maxCharacters = 12000;
			return text.Length > maxCharacters ? text.Substring(0, maxCharacters) : text;
		}

		private List<string> SplitTextIntoChunks(string text, int maxTokens)
		{
			const int charsPerToken = 4;
			int maxChars = maxTokens * charsPerToken;
			List<string> chunks = new List<string>();
			int startIndex = 0;

			while (startIndex < text.Length)
			{
				int chunkSize = Math.Min(maxChars, text.Length - startIndex);
				string chunk = text.Substring(startIndex, chunkSize).Trim();
				chunks.Add(chunk);
				startIndex += chunkSize;
			}

			return chunks;
		}

		private string RemoveNonJsonCharacters(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			int startIndex = 0;
			int endIndex = input.Length;

			while (startIndex < input.Length && !char.IsWhiteSpace(input[startIndex]) && input[startIndex] != '[' && input[startIndex] != '{')
			{
				startIndex++;
			}

			while (endIndex > startIndex && !char.IsWhiteSpace(input[endIndex - 1]) && input[endIndex - 1] != ']' && input[endIndex - 1] != '}')
			{
				endIndex--;
			}

			return input.Substring(startIndex, endIndex - startIndex).Trim();
		}

        public async Task<FlashcardContentAIAssessModel> AssessAnswer(string term, string definition, string userAnswer)
        {
            var activitySource = new ActivitySource("AIService.AssessAnswer");
            using var activity = activitySource.StartActivity("AssessFlashcardAnswer");
            activity?.AddTag("termLength", term?.Length ?? 0);
            activity?.AddTag("definitionLength", definition?.Length ?? 0);
            activity?.AddTag("userAnswerLength", userAnswer?.Length ?? 0);

            if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(definition) || string.IsNullOrWhiteSpace(userAnswer))
            {
                _logger.LogWarning("AssessAnswer called with missing term, definition, or user answer.");
                activity?.SetStatus(ActivityStatusCode.Error, "Missing input");
                return null;
            }

            var systemPrompt = @$"
					You are an AI assistant evaluating a user's answer to a flashcard prompt based on the provided term and definition.
					**First, detect the primary language of the User Answer. Respond ONLY in the detected language. If no language was found, always default to Vietnamese language/**
					Your goal is to assess the correctness and completeness of the user's answer and provide constructive feedback along with a rating based on the FSRS (Free Spaced Repetition Software) scale.

					Input:
					- Term: The concept or question on the flashcard.
					- Definition: The correct or expected answer/explanation for the term.
					- User Answer: The answer provided by the user.

					Output Format:
					Return ONLY a valid JSON object **in the detected language** with the following structure, without any explanation text before or after the JSON:
					{{
					  ""assessment"": ""<string: Your evaluation of the user's answer compared to the definition. Be concise and specific.>"",
					  ""improvement"": ""<string: Provide **hints or guidance** on how the user could improve their answer, focusing on *what type* of information is missing or needs correction, rather than revealing the exact content from the definition. If the answer is perfect, this can be null or an encouraging message. This can never be the same as assessment content>"",
					  ""rating"": <integer: An FSRS rating from 1 to 4 based on the assessment:
								1 (Again): The answer is significantly incorrect or missed the main point.
								2 (Hard): The answer is partially correct but has significant omissions or inaccuracies. Recalling it required considerable effort.
								3 (Good): The answer is largely correct with minor omissions or inaccuracies. Recalling it was relatively easy.
								4 (Easy): The answer is correct, complete, and demonstrates good understanding. Recalling it was effortless.>
					}}

					Example (if English detected):
					{{
					  ""assessment"": ""The user correctly identified the main function but missed mentioning a key side effect."",
					  ""improvement"": ""Consider the potential side effects or consequences of this function for a more complete answer."",
					  ""rating"": 3
					}}

					Example (if Vietnamese detected):
					{{
					  ""assessment"": ""Người dùng đã xác định đúng chức năng chính nhưng bỏ lỡ một tác dụng phụ quan trọng."",
					  ""improvement"": ""Hãy xem xét các tác dụng phụ hoặc hậu quả tiềm ẩn của chức năng này để có câu trả lời đầy đủ hơn."",
					  ""rating"": 3
					}}

					Analyze the provided term, definition, and user answer, detect the language, then return the JSON object adhering strictly to the specified format **in the detected language**.
					";

            var userPrompt = @$"
					Please assess the following flashcard answer:

					Term:
					{term}

					Definition:
					{definition}

					User Answer:
					{userAnswer}

					Provide your assessment in the JSON format specified.
					";

            var chatMessages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            };

            try
            {
                var result = await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_aiSetting.AITimeoutSeconds));
                    var response = await _client.CompleteChatAsync(chatMessages, cancellationToken: cancellationTokenSource.Token);
                    var jsonResponse = response.Value.Content.ElementAtOrDefault(0)?.Text;

                    if (string.IsNullOrWhiteSpace(jsonResponse))
                    {
                        _logger.LogWarning("AssessAnswer: OpenAI response is empty or null for term: {Term}", term);
                        activity?.SetStatus(ActivityStatusCode.Error, ERROR_EMPTY_RESPONSE);
                        return null;
                    }

                    jsonResponse = RemoveNonJsonCharacters(jsonResponse);

                    try
                    {
                        var assessmentResult = JsonConvert.DeserializeObject<FlashcardContentAIAssessModel>(jsonResponse);

                        if (assessmentResult == null || string.IsNullOrWhiteSpace(assessmentResult.Assessment) || assessmentResult.Rating < 1 || assessmentResult.Rating > 4)
                        {
                            _logger.LogWarning("AssessAnswer: Deserialized assessment is invalid or incomplete. JSON: {JsonResponse}", jsonResponse);
                            activity?.SetStatus(ActivityStatusCode.Error, ERROR_INVALID_RESPONSE + ": Incomplete data");
                            return null;
                        }
                        _logger.LogInformation("AssessAnswer: Successfully assessed answer for term: {Term}", term);
                        activity?.SetStatus(ActivityStatusCode.Ok);
                        return assessmentResult;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "AssessAnswer: Failed to deserialize OpenAI JSON response: {Response}", jsonResponse);
                        activity?.SetStatus(ActivityStatusCode.Error, ERROR_INVALID_RESPONSE);
                        return null;
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AssessAnswer: Exception occurred during OpenAI API call or processing for term: {Term}", term);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

                if (ex.Message.Contains("API key", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("authentication", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                if (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                if (ex is OperationCanceledException || ex is TimeoutException)
                {
                    return null;
                }

                return null;
            }
        }
    }
}