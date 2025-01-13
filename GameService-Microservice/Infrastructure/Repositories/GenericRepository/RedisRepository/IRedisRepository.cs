using Domain.Models.Common;
using Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.GenericRepository
{
    public interface IRedisRepository<T> : IRepository<T> where T : class
    {
        Task<PagedList<T>> GetAll(int page, int eachPage,
                                            string sortBy, bool isAscending = false,
                                            params Expression<Func<T, object>>[]? includeProperties);
        Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate,
                                                int page, int eachPage,
                                                string sortBy, bool isAscending = true,
                                                params Expression<Func<T, object>>[]? includeProperties);
    }
}
