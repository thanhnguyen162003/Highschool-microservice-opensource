namespace Infrastructure.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
	IQueryable<T> GetAll();
	Task<T?> GetByIdAsync(dynamic? id);
	Task AddAsync(T entity);
	Task AddRangeAsync(List<T> entities);
	void Update(T entity);
	void UpdateRange(List<T> entities);
    Task DeleteAsync(object id);

}