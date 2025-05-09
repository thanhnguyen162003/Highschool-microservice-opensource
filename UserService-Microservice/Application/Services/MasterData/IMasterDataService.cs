using Application.Common.Models.MasterDataModel;

namespace Application.Services.MasterData
{
    public interface IMasterDataService
    {
        Task<IEnumerable<Cirriculum>> GetCirriculum();
    }
}
