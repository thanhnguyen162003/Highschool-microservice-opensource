using System.Net;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.FlashcardFeatureModel;
using Application.Common.UUID;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services
{
	public class FlashcardStudyService(IUnitOfWork unitOfWork) : IFlashcardStudyService
	{
		public async Task<StudyProgressModel> GetStudyProgress(Guid userId, Guid flashcardId)
		{
			var flashcardContents = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(flashcardId);
			var totalTerms = flashcardContents.Count;

			// Retrieve user progress for the specific flashcard set
			var userProgress = await unitOfWork.UserFlashcardProgressRepository.GetProgressByUser(userId, flashcardId);
			List<Guid> flashcardDoneIds = new List<Guid>();
			List<Guid> flashcardStudyIds = new List<Guid>();
			foreach (var progress in userProgress.Where(x=>x.IsMastered is true))
			{
				flashcardDoneIds.Add(progress.FlashcardContentId);
			}
			foreach (var progress in userProgress.Where(x=>x.IsMastered is false))
			{
				flashcardStudyIds.Add(progress.FlashcardContentId);
			}
			// Determine how many terms have been mastered
			var masteredTerms = userProgress.Count(progress => progress.IsMastered);
			var studyingTerm = userProgress.Count(progress => progress.IsMastered == false);
			var masteredFlashcard = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByIds(flashcardDoneIds);
			var studyingFlashcard = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByIds(flashcardStudyIds);
			var finalMasteredFlashcardContentData =
				masteredFlashcard.Select(x => new StudyProgressModel.MasteredTerm { Id = x.Id, Term = x.FlashcardContentTerm, Definition = x.FlashcardContentDefinition });
			var finalStudyingFlashcardContentData =
				studyingFlashcard.Select(x => new StudyProgressModel.StudyingTerm { Id = x.Id, Term = x.FlashcardContentTerm, Definition = x.FlashcardContentDefinition });
			// Calculate the percentage of mastered terms
			var masteryPercentage = totalTerms > 0 ? ((double)masteredTerms / totalTerms) * 100 : 0;
			
			return new StudyProgressModel
			{
				MasteredTerms = masteredTerms,
				TotalTerms = totalTerms,
				StudyingTermNumber = studyingTerm,
				UnLearnTerm = totalTerms - masteredTerms,
				MasteryPercentage = masteryPercentage,
				StudyingTerms = finalStudyingFlashcardContentData.ToList(),
				MasteredTermsDetail = finalMasteredFlashcardContentData.ToList()
			};
		}
		public async Task<ResponseModel> ResetProgress(Guid userId, Guid flashcardId)
		{
			var userProgress = await unitOfWork.UserFlashcardProgressRepository.GetProgressByUser(userId, flashcardId);
			if (userProgress is null)
			{
				return new ResponseModel(HttpStatusCode.OK,"No progress found");
			}
			await unitOfWork.BeginTransactionAsync();
			foreach (var progress in userProgress)
			{
				var result = await unitOfWork.UserFlashcardProgressRepository.ResetProgressByUserProgressId(progress.Id);
				if (result is false)
				{
					await unitOfWork.RollbackTransactionAsync();
					return new ResponseModel(HttpStatusCode.BadRequest, "Error resetting progress");
				}
			}
			await unitOfWork.CommitTransactionAsync();
			return new ResponseModel(HttpStatusCode.OK, "Successfully reset progress successfully");
		}

		public async Task<List<FlashcardContentQuestionModel>> GenerateFlashcardQuestionsPublic(Guid flashcardId, int questionCount = 7)
        {
            // Initialize Random instance within method scope
            var random = new Random();
            var flashcardContents = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(flashcardId);

            var shuffledContents = flashcardContents.OrderBy(x => random.Next()).ToList();

            var questions = new List<FlashcardContentQuestionModel>();

            // Generate the desired number of questions
            for (int i = 0; i < Math.Min(questionCount, shuffledContents.Count); i++)
            {
                var correctTerm = shuffledContents[i];

                // Randomly pick 3 incorrect definitions
                var incorrectDefinitions = flashcardContents
                    .Where(x => x.Id != correctTerm.Id) // Exclude the correct answer
                    .OrderBy(x => random.Next())
                    .Take(3)
                    .Select(x => new AnswerOption { Id = x.Id, Definition = x.FlashcardContentDefinition })
                    .ToList();

                // Ensure we have enough incorrect definitions
                if (incorrectDefinitions.Count < 3) continue;

                // Add the correct answer to the options
                var correctAnswerOption = new AnswerOption
                {
                    Id = correctTerm.Id,
                    Definition = correctTerm.FlashcardContentDefinition
                };
                incorrectDefinitions.Add(correctAnswerOption);

                // Shuffle the options to randomize the correct answer's position
                var options = incorrectDefinitions.OrderBy(x => random.Next()).ToList();

                // Form the question
                var question = new FlashcardContentQuestionModel
                {
                    Term = correctTerm.FlashcardContentTerm,
                    CorrectAnswerId = correctTerm.Id,
                    CorrectAnswer = correctTerm.FlashcardContentDefinition,
                    Options = options
                };

                questions.Add(question);
            }

            return questions;
        }


		public async Task<ResponseModel> UpdateUserProgress(Guid userId, Guid flashcardContentId, Guid flashcardId, bool isCorrect)
		{
			var progress = await unitOfWork.UserFlashcardProgressRepository
				.GetProgressByUserAndContent(userId, flashcardContentId);

			if (progress == null)
			{
				progress = new UserFlashcardProgress
				{
					Id = new UuidV7().Value,
					UserId = userId,
					FlashcardContentId = flashcardContentId,
					FlashcardId = flashcardId,
					CorrectCount = isCorrect ? 1 : 0,
					LastStudiedAt = DateTime.UtcNow,
					IsMastered = false,
					EaseFactor = 2.5,  // Default initial E-Factor
					Interval = 1,      // Start interval as 1 session
					RepetitionCount = isCorrect ? 1 : 0
				};

				var result = await unitOfWork.UserFlashcardProgressRepository.AddAsync(progress);
				if (result is false)
				{
					return new ResponseModel(HttpStatusCode.BadRequest, "Failed to create user flashcard progress record");
				}
			}
			else
			{
				// Apply SM2 adjustments based on the answer correctness
				UpdateSM2Parameters(progress, isCorrect);

				progress.LastStudiedAt = DateTime.UtcNow;
				
				// Check if the term is "mastered" (e.g., correct 3 times in a row)
				if (progress.RepetitionCount >= 3)
				{
					progress.IsMastered = true;
				}
				var result = await unitOfWork.UserFlashcardProgressRepository.UpdateAsync(progress);
				if (result is false)
				{
					return new ResponseModel(HttpStatusCode.BadRequest, $"Failed to update user progress for user {userId}");
				}
			}
			return new ResponseModel(HttpStatusCode.OK, "Success");
		}

		public async Task<FlashcardQuestionResponseModel> GenerateFlashcardQuestions(Guid flashcardId, Guid userId, int questionCount = 7)
		{
		    var random = new Random();
		    
		    var flashcardContents = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(flashcardId);
		    if (!flashcardContents.Any())
		    {
		        return new FlashcardQuestionResponseModel();
		    }

		    var userProgress = await unitOfWork.UserFlashcardProgressRepository.GetProgressByUser(userId, flashcardId);

		    // Dictionary to map progress to flashcard contents
		    var progressDictionary = userProgress.ToDictionary(progress => progress.FlashcardContentId);

		    // Filter unmastered contents
		    var unmasteredContents = flashcardContents
		        .Where(content => !progressDictionary.TryGetValue(content.Id, out var progress) || !progress.IsMastered)
		        .ToList();
		    
		    var answerContents = flashcardContents
			    .ToList();
		    
		    if (!unmasteredContents.Any())
		    {
		        return new FlashcardQuestionResponseModel
		        {
		            Questions = new List<FlashcardContentQuestionModel>(),
		            NeedPayment = userProgress.Count(progress => progress.IsMastered) > 7
		        };
		    }

		    // Prioritize contents using fetch-based spaced repetition logic
		    var prioritizedContents = unmasteredContents
		        .Select(content =>
		        {
		            var progress = progressDictionary.ContainsKey(content.Id) ? progressDictionary[content.Id] : null;
		            return new
		            {
		                Content = content,
		                Priority = CalculateFetchBasedPriority(progress)
		            };
		        })
		        .OrderBy(x => x.Priority) // Lower priority indicates higher urgency
		        .Take(questionCount)
		        .Select(x => x.Content)
		        .ToList();

		    // Generate questions
		    var questions = new List<FlashcardContentQuestionModel>();
		    foreach (var correctTerm in prioritizedContents)
		    {
		        // Retrieve incorrect definitions excluding the current term
		        var incorrectDefinitions = answerContents
		            .Where(content => content.Id != correctTerm.Id)
		            .OrderBy(_ => random.Next())
		            .Take(3)
		            .ToList();

		        if (incorrectDefinitions.Count < 3) continue;

		        // Build options, including the correct answer
		        var options = incorrectDefinitions
		            .Select(content => new AnswerOption { Id = content.Id, Definition = content.FlashcardContentDefinition })
		            .ToList();

		        options.Add(new AnswerOption
		        {
		            Id = correctTerm.Id,
		            Definition = correctTerm.FlashcardContentDefinition
		        });

		        // Shuffle options
		        options = options.OrderBy(_ => random.Next()).ToList();

		        // Add question to the list
		        questions.Add(new FlashcardContentQuestionModel
		        {
		            Term = correctTerm.FlashcardContentTerm,
		            CorrectAnswerId = correctTerm.Id,
		            CorrectAnswer = correctTerm.FlashcardContentDefinition,
		            Options = options
		        });
		    }

		    return new FlashcardQuestionResponseModel
		    {
		        Questions = questions,
		        NeedPayment = userProgress.Count(progress => progress.IsMastered) > 7
		    };
		}

		private double CalculateFetchBasedPriority(UserFlashcardProgress? progress)
		{
		    if (progress == null)
		    {
		        // New terms are assigned the highest priority
		        return 0.0;
		    }

		    // Fetch-based priority calculation using intervals and correct/incorrect answers
		    // Priority = Interval / (RepetitionCount + 1)
		    // Lower values indicate higher urgency
		    return progress.Interval / (progress.RepetitionCount + 1.0);
		}



		private void UpdateSM2Parameters(UserFlashcardProgress progress, bool isCorrect)
		{
			if (isCorrect)
			{
				// Increase repetition count on correct answer
				progress.RepetitionCount++;
				progress.CorrectCount++;
				// Update interval based on the SM2 algorithm
				if (progress.RepetitionCount == 1)
				{
					progress.Interval = 1; // First correct answer sets interval to 1
				}
				else if (progress.RepetitionCount == 2)
				{
					progress.Interval = 2; // Second correct answer sets interval to 2
				}
				else
				{
					// Calculate the next interval using E-Factor
					progress.Interval = (int)(progress.Interval * progress.EaseFactor);
				}

				// Adjust the E-Factor based on user performance (correct answer)
				progress.EaseFactor = Math.Max(1.3, progress.EaseFactor - 0.15 + (0.1 * (5 - 4))); // 4 is the assumed response quality for correct answers
			}
			else
			{
				// Reset repetition count on incorrect answer
				progress.RepetitionCount = 0;
			
				// Reset interval to 1
				progress.Interval = 1;

				// Decrease the E-Factor for incorrect answers
				progress.EaseFactor = Math.Max(1.3, progress.EaseFactor - 0.2);
			}
		}
	}
}
