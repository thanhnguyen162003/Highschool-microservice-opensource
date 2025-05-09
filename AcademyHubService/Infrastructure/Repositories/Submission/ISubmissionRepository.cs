using Domain.Entity;
using Domain.Models.Common;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface ISubmissionRepository : ISqlRepository<Submission>
    {
        Task<Submission> GetSubmission(Guid assignmentId, int memberId);
        Task<List<Submission>> GetSubmissionForTeacher(Guid assignmentId);
        Task<List<Submission>> GetSubmissionForStudent(Guid assignmentId, int memberId);
    }
}