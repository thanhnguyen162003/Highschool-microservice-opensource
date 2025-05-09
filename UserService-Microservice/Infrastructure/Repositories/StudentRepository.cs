using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class StudentRepository(UserDatabaseContext context) : BaseRepository<Student>(context), IStudentRepository
{
	private readonly UserDatabaseContext _context = context;

    public Task<bool> AddUser(Student student)
	{
		throw new NotImplementedException();
	}

	public async Task<Student?> GetStudentByUserId(Guid userId)
	{
		return await _context.Students
			.Include(x => x.BaseUser).ThenInclude(x => x.UserSubjects)
			.FirstOrDefaultAsync(x => x.BaseUserId.Equals(userId));
	}
    public async Task<int> GetTotalStudentAmount()
    {
        return await _entities
			.Include(x => x.BaseUser)
            .Where(x => x.BaseUser.DeletedAt == null).CountAsync();
    }
	public async Task<bool> UpdateStudent(Student student)
	{
		_context.Entry(student).State = EntityState.Modified;
		var rowsAffected = await _context.SaveChangesAsync();
		return rowsAffected > 0;
	}
}