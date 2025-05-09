namespace Infrastructure.Repositories.Interfaces;

public interface ITeacherRepository : IRepository<Teacher>
{
	Task<Teacher?> GetTeacherByUserId(Guid userId);
    Task<int> GetTotalTeacherAmount();
    Task<Dictionary<string, int>> GetTeacherExperienceCount();
}