using Infrastructure.Repositories.Interfaces;
using Infrastructure.Contexts;
using Domain.Entities;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class EnrollmentRepository(DocumentDbContext context) : BaseRepository<Enrollment>(context), IEnrollmentRepository
    {
        public async Task<Dictionary<string, string>> GetSubjectAndUserEnroll (IEnumerable<string> userIds, IEnumerable<Guid> subjectIds)
        {
            var list = await _entities
                .Where(e => userIds.Contains(e.BaseUserId.ToString()) && subjectIds.Contains(e.SubjectCurriculumId))
                .ToListAsync();

            Dictionary<string, string> result = new Dictionary<string, string>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    result.Add(item.SubjectCurriculumId.ToString(), item.BaseUserId.ToString());
                }

            }
            return result;
        }

    }
}
