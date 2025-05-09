using Domain.Entities;
using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface ICurriculumRepository : IRepository<Curriculum>
{
    Task<(List<Curriculum> Curricula, int TotalCount)> GetAllCurriculumsAsync(CurriculumQueryFilter queryFilter);
    Task<bool> DeleteCurriculum(Guid id);
    Task<List<Curriculum>> GetAll();
	Task<List<Curriculum>> GetAllNotExternal();
	Task<Curriculum> GetCurriculumById(Guid id);
}