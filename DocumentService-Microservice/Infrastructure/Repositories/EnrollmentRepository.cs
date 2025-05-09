using Infrastructure.Repositories.Interfaces;
using Infrastructure.Contexts;
using Domain.Entities;
using System.Linq;
using Domain.CustomModel;

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
        public async Task<List<UserLessonLearningModel>> GetEnrollmentCount()
        {
            var result = await _entities
                .Include(e => e.EnrollmentProgresses)
                .Select( e => new UserLessonLearningModel
                {
                    UserId= e.BaseUserId,
                    LessonLearnDate = e.EnrollmentProgresses.Select(ep => ep.CreatedAt).ToList(),
                })
                .ToListAsync();

            return result;
        }
        public async Task<int> GetEnrollAmount()
        {
            var count = await _entities.GroupBy(x => x.BaseUserId).CountAsync();
            return count;
        }
        public async Task<List<TopEnrolledSubjectModel>> GetEnrollmentCompletionStatus()
        {
            // Get top enrolled subjects
            var enrollments = await _entities
                .AsNoTracking()
                .Include(e => e.SubjectCurriculum)
                .GroupBy(e => e.SubjectCurriculumId)
                .Select(g => new
                {
                    SubjectCurriculumId = g.Key,
                    SubjectCurriculumName = g.First().SubjectCurriculum.SubjectCurriculumName,
                    TotalEnrollmentCount = g.Count()
                })
                .OrderByDescending(s => s.TotalEnrollmentCount)
                .Take(5)
                .ToListAsync();

            // Get required lessons per subject
            var requiredLessons = await _entities
                .Where(e => enrollments.Select(en => en.SubjectCurriculumId).Contains(e.SubjectCurriculumId))
                .Select(e => new
                {
                    e.SubjectCurriculumId,
                    LessonId = e.SubjectCurriculum.Chapters
                        .SelectMany(c => c.Lessons)
                        .Select(l => l.Id)
                })
                .ToListAsync();

            // Convert to dictionary for quick lookup
            var requiredLessonMap = requiredLessons
                .GroupBy(l => l.SubjectCurriculumId)
                .ToDictionary(g => g.Key, g => g.SelectMany(l => l.LessonId).ToHashSet());

            // Get user progress
            var userProgress = await _entities
                .Where(e => requiredLessonMap.Keys.Contains(e.SubjectCurriculumId))
                .Select(e => new
                {
                    e.BaseUserId,
                    e.SubjectCurriculumId,
                    LessonId = e.EnrollmentProgresses.Select(ep => ep.LessonId)
                })
                .ToListAsync();

            // Group progress by user & subject
            var userCompletionMap = userProgress
                .GroupBy(p => new { p.BaseUserId, p.SubjectCurriculumId })
                .ToDictionary(g => g.Key, g => g.SelectMany(p => p.LessonId).ToHashSet());

            // Compute completion percentage
            var topSubjects = enrollments.Select(subject =>
            {
                int completedUsers = userCompletionMap
                    .Where(kvp => kvp.Key.SubjectCurriculumId == subject.SubjectCurriculumId)
                    .Count(kvp => requiredLessonMap[subject.SubjectCurriculumId].IsSubsetOf(kvp.Value));

                return new TopEnrolledSubjectModel
                {
                    Name = subject.SubjectCurriculumName,
                    TotalEnrollmentCount = subject.TotalEnrollmentCount,
                    Completion = (int)((completedUsers / (double)subject.TotalEnrollmentCount) * 100)
                };
            }).ToList();

            return topSubjects;
        }
    }
}
