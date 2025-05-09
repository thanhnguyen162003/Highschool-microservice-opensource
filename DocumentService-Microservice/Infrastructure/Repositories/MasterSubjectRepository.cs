using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class MasterSubjectRepository(DocumentDbContext context) : BaseRepository<MasterSubject>(context), IMasterSubjectRepository
{
    public async Task<bool> AddMasterSubject(MasterSubject masterSubject)
    {
        var result = await context.Database.ExecuteSqlRawAsync(
            "INSERT INTO \"MasterSubject\" (Id, \"masterSubjectName\", \"masterSubjectSlug\", \"createdAt\", \"updatedAt\") VALUES ({0}, {1}, {2}, {3}, {4})",
            masterSubject.Id, masterSubject.MasterSubjectName, masterSubject.MasterSubjectSlug, masterSubject.CreatedAt, masterSubject.UpdatedAt
        );
        return result > 0;
    }

    public async Task<bool> UpdateMasterSubject(MasterSubject masterSubject)
    {
        var result = await context.Database.ExecuteSqlRawAsync(
            "UPDATE \"MasterSubject\" SET \"masterSubjectName\" = {1}, \"masterSubjectSlug\" = {2}, \"updatedAt\" = {3} WHERE Id = {0}",
            masterSubject.Id, masterSubject.MasterSubjectName, masterSubject.MasterSubjectSlug, masterSubject.UpdatedAt
        );
        return result > 0;
    }

    public async Task<bool> DeleteMasterSubject(Guid id)
    {
        var result = await context.Database.ExecuteSqlRawAsync(
            "DELETE FROM \"MasterSubject\" WHERE Id = {0}", id
        );
        return result > 0;
    }

    public async Task<MasterSubject> GetMasterSubjectById(Guid id)
    {
        return (await context.MasterSubjects
            .FromSqlRaw("SELECT * FROM \"MasterSubject\" WHERE Id = {0}", id)
            .FirstOrDefaultAsync())!;
    }

    public async Task<(List<MasterSubject> MasterSubjects, int TotalCount)> GetMasterSubjects(
    MasterSubjectQueryFilter queryFilter)
    {
        var masterSubjects = _entities
            .AsNoTracking()
            .AsQueryable();

        int totalCount = await masterSubjects.CountAsync();

        masterSubjects = masterSubjects
            .OrderBy(ms => ms.MasterSubjectName)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        var pagination = await masterSubjects.ToListAsync();
        return (pagination, totalCount);
    }
	public async Task<IEnumerable<string>> CheckMasterSubjectName(IEnumerable<string> masterSubjectIds)
	{
		var existingMasterSubjectNamesInDb = await _entities
			.Select(x => x.Id.ToString())
			.ToListAsync();

		var nonExistingMasterSubjectNames = masterSubjectIds.Except(existingMasterSubjectNamesInDb);

		return nonExistingMasterSubjectNames;

	}
}