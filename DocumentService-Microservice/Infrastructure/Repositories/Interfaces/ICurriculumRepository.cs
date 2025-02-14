namespace Infrastructure.Repositories.Interfaces;

public interface ICurriculumRepository : IRepository<Curriculum>
{
    Task<List<Curriculum>> GetAllCurriculumsAsync();
}