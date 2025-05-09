using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class TeacherRepository(UserDatabaseContext context) : BaseRepository<Teacher>(context), ITeacherRepository
{
	private readonly UserDatabaseContext _context = context;

    public async Task<Teacher?> GetTeacherByUserId(Guid userId)
	{
		return await _context.Teachers
			.Include(x => x.Certificates)
			.FirstOrDefaultAsync(x => x.BaseUserId.Equals(userId));
	}
    public async Task<int> GetTotalTeacherAmount()
    {
        return await _entities
            .Include(x => x.BaseUser)
            .Where(x => x.BaseUser.DeletedAt == null).CountAsync();
    }
    public async Task<Dictionary<string, int>> GetTeacherExperienceCount()
    {
        var teachers = await _entities
            .Where(x => x.BaseUser.DeletedAt == null)
            .Select(t => t.ExperienceYears)
            .ToListAsync();

        var experienceRanges = new Dictionary<string, Func<int, bool>>
    {
        { "0-2", years => years >= 0 && years <= 2 },
        { "3-5", years => years >= 3 && years <= 5 },
        { "6-10", years => years >= 6 && years <= 10 },
        { "11-15", years => years >= 11 && years <= 15 },
        { "16+", years => years >= 16 }
    };

        var result = experienceRanges.ToDictionary(
            range => range.Key,
            range => teachers.Count(t => range.Value(t))
        );

        return result;
    }
}