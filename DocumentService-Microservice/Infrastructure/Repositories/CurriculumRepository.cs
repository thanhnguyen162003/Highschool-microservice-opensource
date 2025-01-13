using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class CurriculumRepository(DocumentDbContext context) : BaseRepository<Curriculum>(context), ICurriculumRepository
{
    public async Task<List<Curriculum>> GetAllCurriculumsAsync()
    {
        return await _entities.ToListAsync();
    }
}