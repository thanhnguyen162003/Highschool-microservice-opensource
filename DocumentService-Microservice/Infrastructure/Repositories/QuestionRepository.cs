using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class QuestionRepository(DocumentDbContext context) : BaseRepository<Question>(context), IQuestionRepository
    {
        public async Task<(List<Question> Questions, int TotalCount)> GetQuestionAdvanceFilter(
    QuestionAdvanceQueryFilter queryFilter, CancellationToken cancellationToken = default)
        {
            IQueryable<Question> query = _entities.Include(q => q.QuestionAnswers);

            if (queryFilter.LessonId.HasValue)
            {
                query = query.Where(q => q.LessonId == queryFilter.LessonId);
            }

            if (queryFilter.ChapterId.HasValue)
            {
                query = query.Where(q => q.ChapterId == queryFilter.ChapterId);
            }

            if (queryFilter.SubjectCurriculumId.HasValue)
            {
                query = query.Where(q => q.SubjectCurriculumId == queryFilter.SubjectCurriculumId);
            }

            if (queryFilter.SubjectId.HasValue)
            {
                query = query.Where(q => q.SubjectId == queryFilter.SubjectId);
            }

            if (queryFilter.Difficulty.HasValue)
            {
                query = query.Where(q => q.Difficulty == queryFilter.Difficulty);
            }

            if (queryFilter.QuestionType.HasValue)
            {
                query = query.Where(q => q.QuestionType == queryFilter.QuestionType);
            }

            if (queryFilter.Category.HasValue)
            {
                query = query.Where(q => q.Category == queryFilter.Category);
            }

            if (!string.IsNullOrEmpty(queryFilter.Search))
            {
                query = query.Where(q => q.QuestionContent.Contains(queryFilter.Search));
            }

            int totalCount = await query.CountAsync(cancellationToken);

            if (queryFilter.PageSize > 0)
            {
                query = query
                    .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
                    .Take(queryFilter.PageSize);
            }

            var questions = await query.ToListAsync(cancellationToken);
            return (questions, totalCount);
        }


    }
}
