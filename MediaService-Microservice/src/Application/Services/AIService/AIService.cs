using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Application.Common.Models;
using Application.Common.Models.AIModels;
using Application.Common.Models.QuizModel;
using Application.Constants;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Services.AIService;

public class AIService(IOptions<DocumentAISetting> options) : IAIService
{
    private readonly DocumentAISetting _documentAISetting = options.Value;

    private string GenerateScript(string url)
    {
        var script = new StringBuilder($"Please analyze the content and generate at least 5 multiple-choice questions with 4 answer options (A, B, C, D), including the correct answer for each question. " +
            $"The questions must closely reflect the information from the document.\n" +
            $"This is a URL of document '{url}'\n" +
            $"Return the questions an list object with model:\n" +
            $"{new QuestionModel()}");

        return script.ToString();
    }

    public async Task<IEnumerable<QuestionModel>> GenerateQuiz(string url)
    {
        Console.WriteLine("GenerateQuiz");
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add(DocumentAIConstants.CozeAPI.Authorization, $"Bearer {_documentAISetting.Authorization}");
        client.DefaultRequestHeaders.Add(DocumentAIConstants.CozeAPI.Connection, _documentAISetting.Connection);

        var data = new
        {
            bot_id = _documentAISetting.BotId,
            stream = _documentAISetting.Stream,
            user = _documentAISetting.User,
            query = GenerateScript(url)
        };

        var response = await client.PostAsJsonAsync(_documentAISetting.API, data);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JObject.Parse(responseContent);
            var result = responseData[DocumentAIConstants.CozeProperty.Messages]?.FirstOrDefault(x => x[DocumentAIConstants.CozeProperty.Type]!.ToString().Equals(DocumentAIConstants.CozeProperty.Answer))?[DocumentAIConstants.CozeProperty.Content];

            try
            {
                result = ExtractJsonArray(result!.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", e.Message);
                return new List<QuestionModel>();
            }

            if (CheckNull(result!.ToString()) || IsJson(result!.ToString()))
            {
                return JsonConvert.DeserializeObject<IEnumerable<QuestionModel>>(result!.ToString()) ?? new List<QuestionModel>();
            }
        }

        Console.WriteLine("complete");

        return new List<QuestionModel>();
    }

    private static string ExtractJsonArray(string rawJson)
    {
        // Find the position of the array start ('[') and array end (']')
        int startIndex = rawJson.IndexOf('[');
        int endIndex = rawJson.LastIndexOf(']');

        // If valid positions are found, extract the substring containing the JSON array
        if (startIndex >= 0 && endIndex > startIndex)
        {
            return rawJson.Substring(startIndex, endIndex - startIndex + 1);
        }

        // If the JSON is invalid or no array is found, throw an error
        throw new InvalidOperationException("Invalid JSON format. Unable to find a JSON array.");
    }

    private static bool IsJson(string str)
    {
        try
        {
            JsonDocument.Parse(str);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool CheckNull(string data)
    {
        Regex regex = new("null");
        MatchCollection matches = regex.Matches(data);
        if (matches.Count >= 2)
        {
            return true;
        }
        return false;
    }
}
