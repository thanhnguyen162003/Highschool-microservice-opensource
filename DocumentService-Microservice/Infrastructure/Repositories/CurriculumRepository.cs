using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class CurriculumRepository(DocumentDbContext context) : BaseRepository<Curriculum>(context), ICurriculumRepository
{
	public async Task<bool> DeleteCurriculum(Guid id)
	{
		var result = await context.Database.ExecuteSqlRawAsync(
			"DELETE FROM \"Curriculum\" WHERE Id = {0}", id
		);
		return result > 0;
	}

	public async Task<List<Curriculum>> GetAll()
	{
		return await _entities.AsNoTracking()
			.ToListAsync();
	}
	public async Task<List<Curriculum>> GetAllNotExternal()
	{
		return await _entities.AsNoTracking().Where(x=>x.IsExternal == false)
			.ToListAsync();
	}

	public async Task<(List<Curriculum> Curricula, int TotalCount)> GetAllCurriculumsAsync(CurriculumQueryFilter queryFilter)
    {
		var curricula = _entities
		   .AsNoTracking()
		   .AsQueryable();

		int totalCount = await curricula.CountAsync();

		curricula = curricula
			.OrderBy(ms => ms.CurriculumName)
			.Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
			.Take(queryFilter.PageSize);

		var pagination = await curricula.ToListAsync();
		return (pagination, totalCount);
	}

	public async Task<Curriculum> GetCurriculumById(Guid id)
	{
		return await _entities
			.FirstOrDefaultAsync(x => x.Id == id);
	}
}