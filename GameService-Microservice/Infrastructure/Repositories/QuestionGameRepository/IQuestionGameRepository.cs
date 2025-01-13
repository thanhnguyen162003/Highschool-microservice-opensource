using Domain.Models.PlayGameModels;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface IQuestionGameRepository : IRedisRepository<QuestionGame>
    {
        Task AddQuestions(string roomId, IEnumerable<QuestionGame> questions);
        Task<QuestionGame?> GetQuestion(string roomId, int order);
        Task DeleteQuestion(string roomId);
    }
}
