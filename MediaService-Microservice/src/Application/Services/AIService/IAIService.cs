using Application.Common.Models;
using Application.Common.Models.QuizModel;

namespace Application.Services.AIService;

public interface IAIService
{
    Task<IEnumerable<QuestionModel>> GenerateQuiz(string url);
}
