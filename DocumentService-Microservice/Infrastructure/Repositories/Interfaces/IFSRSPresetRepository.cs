using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IFSRSPresetRepository : IRepository<FSRSPreset>
    {
        Task<List<FSRSPreset>> GetPreset(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> DeletePreset(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UpdatePreset(FSRSPreset fSRSPreset, CancellationToken cancellation);
        Task<bool> CreatePreset(FSRSPreset fSRSPreset, CancellationToken cancellationToken = default);
        Task<(List<FSRSPreset> Presets, int TotalCount)> GetPresetAsync(FSRSPresetQueryFilter queryFilter, Guid userId);
        Task<(List<FSRSPreset> Presets, int TotalCount)> GetPresetAsyncAdmin(FSRSPresetQueryFilter queryFilter);
        Task<FSRSPreset> GetPresetById(Guid id);
        Task<(double[] FsrsParameters, double Retrievability)?> GetPresetParameterAndR(Guid id);
    }
} 