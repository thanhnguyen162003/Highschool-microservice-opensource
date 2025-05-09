using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class BaseRepository<T>(UserDatabaseContext context) : IRepository<T> where T : class
{
	private readonly UserDatabaseContext _context = context;
	protected readonly DbSet<T> _entities = context.Set<T>();

    public async Task<T?> GetByIdAsync(dynamic? id)
	{
		return await _entities.FindAsync(id);
	}

	public async Task AddAsync(T entity)
	{
		await _entities.AddAsync(entity);
	}

	public async Task AddRangeAsync(List<T> entities)
	{
		await _entities.AddRangeAsync(entities);
	}

	public IQueryable<T> GetAll()
	{
		return _entities.AsQueryable();
	}

	public void Update(T entity)
	{
		_entities.Update(entity);
	}

	public void UpdateRange(List<T> entities)
	{
		_entities.UpdateRange(entities);
	}
    // public void DeleteRange(List<T> entities)
    // {
    //     foreach (var entity in entities)
    //     {
    //         entity.IsDeleted = true;
    //         _entities.Update(entity);
    //     }
    //     _context.SaveChanges();
    // }

    public async Task DeleteAsync(object id)
    {
        T? entityToDelete = await _entities.FindAsync(id);
        if (entityToDelete != null)
        {
            Delete(entityToDelete);
        }
    }

    public virtual void Delete(T entityToDelete)
    {
        if (_context.Entry(entityToDelete).State == EntityState.Detached)
        {
            _entities.Attach(entityToDelete);
        }
        _entities.Remove(entityToDelete);
    }
}