using System.Linq.Expressions;

public interface IRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> Get(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int? skipCount = 0,
        int? takeCount = 0,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includeProperties);

    IQueryable<TEntity> GetQueryable(
    Expression<Func<TEntity, bool>>? filter = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    params Expression<Func<TEntity, object>>[]? includeProperties);

    TEntity? GetByID(object id);
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken);
    TEntity Insert(TEntity entity);
    Task<TEntity> InsertAsync(TEntity entity);
    Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    Task DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken);
    Task DeleteAsync(object id);
    void Delete(TEntity entityToDelete);
    TEntity Update(TEntity entityToUpdate);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);
}
