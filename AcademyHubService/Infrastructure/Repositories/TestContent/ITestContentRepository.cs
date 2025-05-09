using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface ITestContentRepository : ISqlRepository<TestContent>
    {
        Task CreateTestContent(List<TestContent> test);
        Task Delete(IEnumerable<TestContent> testContents);
        Task<Dictionary<Guid, string>> SubmitTest(List<TestContent> test, Guid assignmentId);
    }
}
