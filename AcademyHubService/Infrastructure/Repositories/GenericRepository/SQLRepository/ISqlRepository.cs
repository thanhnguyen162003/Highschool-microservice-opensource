using Domain.Models.Common;
using Repositories.GenericRepository;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.GenericRepository
{
    public interface ISqlRepository<T> : IRepository<T> where T : class
    {
        Task<T?> GetById(dynamic[] id);
        Task UpdateRange(IEnumerable<T> entities);
        Task AddRange(IEnumerable<T> entities);
        Task Delete(dynamic[] id);
        Task<PagedList<T>> GetAll(int page, int eachPage,
                                            string sortBy, bool isAscending = false,
                                            params Expression<Func<T, object>>[]? includeProperties);
        Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate,
                                                int page, int eachPage,
                                                string sortBy, bool isAscending = true,
                                                params Expression<Func<T, object>>[]? includeProperties);

        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[]? includeProperties);
    }
}
