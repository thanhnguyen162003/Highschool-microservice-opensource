using Domain.QueriesFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<(List<Question> Questions, int TotalCount)> GetQuestionAdvanceFilter(QuestionAdvanceQueryFilter queryFilter, CancellationToken cancellationToken = default);
    }
}
