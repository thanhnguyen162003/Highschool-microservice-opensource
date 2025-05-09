using Domain.Common;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class BaseRepository<TEntity>(DocumentDbContext context) : IRepository<TEntity> where TEntity : class
{
    private readonly DocumentDbContext _context = context;
    protected readonly DbSet<TEntity> _entities = context.Set<TEntity>();

    //EXAMPLE ORDERBY : orderBy: q => q.OrderBy(d => d.Name)
    public async Task<IEnumerable<TEntity>> Get(
    Expression<Func<TEntity, bool>>? filter = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    int? skipCount = 0,
    int? takeCount = 0,
    CancellationToken cancellationToken = default,
    params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        IQueryable<TEntity> query = _entities;

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (skipCount > 0)
        {
            query = query.Skip(skipCount.Value);
        }

        if (takeCount > 0)
        {
            query = query.Take(takeCount.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public IQueryable<TEntity> GetQueryable(
    Expression<Func<TEntity, bool>>? filter = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        IQueryable<TEntity> query = _entities;

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query;
    }



    public virtual TEntity? GetByID(object id)
    {
        return _entities.Find(id);
    }
    public virtual async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken)
    {
        return await _entities.FindAsync(id, cancellationToken);
    }


    public virtual TEntity Insert(TEntity entity)
    {
        _entities.Add(entity);
        return entity;
    }

    public virtual async Task<TEntity> InsertAsync(TEntity entity)
    {
        var _ = await _entities.AddAsync(entity);
        return entity;
    }

    public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await _entities.AddRangeAsync(entities, cancellationToken);
    }

    public async Task DeleteAsync(object id)
    {
        TEntity? entityToDelete = await _entities.FindAsync(id);
        if (entityToDelete != null)
        {
            Delete(entityToDelete);
        }
    }
    public async Task DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken)
    {
        var entitiesToDelete = await _entities.Where(filter).ToListAsync(cancellationToken);

        if (entitiesToDelete.Any())
        {
            _entities.RemoveRange(entitiesToDelete);
        }
    }


    public virtual void Delete(TEntity entityToDelete)
    {
        if (_context.Entry(entityToDelete).State == EntityState.Detached)
        {
            _entities.Attach(entityToDelete);
        }
        _entities.Remove(entityToDelete);
    }

    public virtual TEntity Update(TEntity entityToUpdate)
    {
        _entities.Attach(entityToUpdate);
        _entities.Entry(entityToUpdate).State = EntityState.Modified;
        return entityToUpdate;
    }
    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? filter = null)
    {
        IQueryable<TEntity> query = _entities;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync();
    }
}