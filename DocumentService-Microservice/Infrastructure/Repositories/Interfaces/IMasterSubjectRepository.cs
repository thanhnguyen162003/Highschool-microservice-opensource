using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface IMasterSubjectRepository : IRepository<MasterSubject>
{
    Task<bool> AddMasterSubject(MasterSubject masterSubject);
    Task<bool> UpdateMasterSubject(MasterSubject masterSubject);
    Task<bool> DeleteMasterSubject(Guid id);
    Task<MasterSubject> GetMasterSubjectById(Guid id);
    Task<(List<MasterSubject> MasterSubjects, int TotalCount)> GetMasterSubjects(MasterSubjectQueryFilter queryFilter);
	Task<IEnumerable<string>> CheckMasterSubjectName(IEnumerable<string> masterSubjectIds);
}

