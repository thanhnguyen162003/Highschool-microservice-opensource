using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface ISchoolRepository : IRepository<School>
{
    Task<bool> CreateSchoolAsync(School school);
    Task<bool> DeleteSchool(Guid id);
    Task<(List<School> Schools, int TotalCount)> GetSchoolsAsync(SchoolQueryFilter queryFilter);
    Task<(List<School> Schools, int TotalCount)> GetSchoolsByProvinceIdAsync(int id, SchoolQueryFilter queryFilter);
}