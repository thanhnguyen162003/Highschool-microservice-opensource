using Domain.Models.Common;
using System.Linq.Expressions;

namespace Repositories.GenericRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetById(dynamic id);
        Task<T?> GetBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[]? includeProperties);
        Task Add(T entity);
        Task Delete(dynamic id);
        Task Update(T entity);
        Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[]? includeProperties);
        Task<PagedList<T>> GetAll(int page, int eachPage, params Expression<Func<T, object>>[]? includeProperties);
        Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate,
                                            int page, int eachPage,
                                            params Expression<Func<T, object>>[]? includeProperties);
    }
}
