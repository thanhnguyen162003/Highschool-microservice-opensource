using Domain.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<Dictionary<string, string>> GetSubjectAndUserEnroll(IEnumerable<string> userIds, IEnumerable<Guid> subjectIds);
        Task<List<TopEnrolledSubjectModel>> GetEnrollmentCompletionStatus();
        Task<int> GetEnrollAmount();
        Task<List<UserLessonLearningModel>> GetEnrollmentCount();
    }
}
