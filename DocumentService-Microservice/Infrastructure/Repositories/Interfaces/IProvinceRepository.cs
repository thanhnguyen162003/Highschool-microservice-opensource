using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface IProvinceRepository : IRepository<Province>
{
    Task<bool> CreateProvinceAsync(Province province);
    Task<bool> DeleteProvince(Guid id);
    Task<(List<Province> Provinces, int TotalCount)> GetProvinceAsync(ProvinceQueryFilter queryFilter);
}