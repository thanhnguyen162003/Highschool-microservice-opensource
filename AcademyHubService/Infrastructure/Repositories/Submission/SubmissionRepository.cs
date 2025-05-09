using Domain.Entity;
using Domain.Models.Common;
using Infrastructure.Extensions;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories
{
    public class SubmissionRepository(DbContext context) : SqlRepository<Submission>(context), ISubmissionRepository
    {
        public async Task<Submission> GetSubmission(Guid assignmentId, int memberId)
        {
            var query = await _dbSet.AsQueryable()
                .FirstOrDefaultAsync(x => x.AssignmentId.Equals(assignmentId) && x.MemberId.Equals(memberId));
            return query;
        }
        public async Task<List<Submission>> GetSubmissionForTeacher(Guid assignmentId)
        {
            var query = await _dbSet
                .Where(x => x.AssignmentId.Equals(assignmentId))
                .Include(x => x.Member)
                .ToListAsync();
            return query;
        }
        public async Task<List<Submission>> GetSubmissionForStudent(Guid assignmentId, int memberId)
        {
            var query = await _dbSet
                .Where(x => x.AssignmentId.Equals(assignmentId) && x.MemberId.Equals(memberId))
                .Include(x => x.Member)
                .ToListAsync();
            return query;
        }
    }
}
