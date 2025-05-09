using Domain.Common.Models;
using Domain.Enumerations;
using Infrastructure.Extensions;
using Infrastructure.Repositories.Interfaces;


namespace Infrastructure.Repositories;

public class UserRepository(UserDatabaseContext context) : BaseRepository<BaseUser>(context), IUserRepository
{
    public async Task<Dictionary<string, int>> CountUserByRole(IEnumerable<string> userIds)
    {
        var roleCounts = await _entities
        .Where(x => userIds.Contains(x.Id.ToString()))
        .Include(s => s.Role)  // Include the Role entity
        .GroupBy(x => x.Role.RoleName)  // Group by RoleName
        .Select(g => new
        {
            RoleName = g.Key,   // string
            Count = g.Count()   // int
        })
        .ToListAsync();

        // Convert to Dictionary<string, int>
        return roleCounts.ToDictionary(
            x => x.RoleName ?? "Unknown",  // Handle null RoleName
            x => x.Count
        );
    }
    public async Task<int> GetTotalUserAmount()
    {
        return await _entities.CountAsync();
    }
    public async Task<int> GetUserAmountCreateThisMonth()
    {
        return await _entities.CountAsync(x => x.CreatedAt >= new DateTime(DateTime.Now.Year, DateTime.Now.Month,1,0,0,0));
    }
    public async Task<int> GetUserAmountCreateLastMonth()
    {
        var test = DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month);
        return await _entities.CountAsync(x => x.CreatedAt >= new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1, 0, 0, 0) && x.CreatedAt <= new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, test, 23, 59, 59));
    }
    public async Task<int> GetTotalActiveUserAmount()
    {
        return await _entities.Where(x => x.DeletedAt == null && x.Status.Equals(AccountStatus.Active.ToString())).CountAsync();
    }
    public async Task<int> GetTotalBlockedUserAmount()
    {
        return await _entities.Where(x => x.DeletedAt == null && x.Status.Equals(AccountStatus.Blocked.ToString())).CountAsync();
    }
    public async Task<int> GetTotalDeletedUserAmount()
    {
        return await _entities.Where(x => x.DeletedAt != null && x.Status.Equals(AccountStatus.Deleted.ToString())).CountAsync();
    }

    public async Task<BaseUser?> GetUserByUsernameOrEmail(string username)
	{
		return await _entities
			.Where(x => (x.Username!.Equals(username) || x.Email!.Equals(username)))
			.Include(r => r.Role)
			.Include(r => r.UserSubjects)
			.Include(r => r.Student)
			.Include(r => r.Teacher)
			.FirstOrDefaultAsync();
	}

	public async Task<BaseUser?> GetUserByUserId(Guid userId)
	{
		return await _entities
			.Include(r => r.Role)
			.Where(x => x.Id == userId && x.DeletedAt == null)
			.FirstOrDefaultAsync();
	}

	public async Task<BaseUser?> GetUserByUsername(string username)
	{
		return await _entities
			.Where(x => (x.Username.Equals(username)) || x.Email.Equals(username))
			.Include(r => r.Role)
			.FirstOrDefaultAsync();
	}

	public async Task<BaseUser?> GetUser(string key)
	{
		return await _entities
			.Include(r => r.Role)
			.FirstOrDefaultAsync(x => x.Username.Equals(key) || x.Email.Equals(key) || x.Id.ToString().Equals(key));

    }

	public async Task<bool> IsExistEmail(string email)
	{
		return await _entities.AnyAsync(x => x.Email!.Equals(email));
	}

	public async Task<bool> IsExistUserName(string username, Guid userId)
	{
		return await _entities.AnyAsync(x => x.Username!.Equals(username) && !x.Id.Equals(userId));
	}

	public async Task<int?> GetRoleId(string username)
	{
		return await _entities.Where(x => x.Username.Equals(username))
			.Select(x => x.RoleId)
			.FirstOrDefaultAsync();
	}

	public async Task<BaseUser?> GetDetailUser(Guid userId)
	{
		return await _entities
			.Where(x => x.Id.Equals(userId) && x.DeletedAt == null)
			.Include(r => r.Role)
			.Include(r => r.UserSubjects)
			.Include(r => r.Student)
            .Include(r => r.Teacher)
            .FirstOrDefaultAsync();
	}
    public async Task<List<BaseUser>> GetUserForMedia()
    {
        return await _entities
            .Where(x => x.RoleId == 1 || x.RoleId == 2 || x.RoleId == 3 && x.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<PagedList<BaseUser>> GetAllUser(int page, int eachPage, int roleId, string? search, IEnumerable<string>? status)
	{
		var query = _entities
			.Include(e => e.Student)
			.Include(e => e.Teacher)
			.Include(r => r.Role)
			.Where(x => !x.RoleId!.Equals((int)RoleEnum.Admin)
					&& (x.RoleId == roleId))
			.AsSplitQuery();

		if (!string.IsNullOrEmpty(search))
		{
			query = query.Where(x => x.Username!.Contains(search) || x.Email!.Contains(search) || x.Fullname!.Contains(search));
		}

		if (status.Any())
		{
			query = query.Where(x => status!.Contains(x.Status));
		}

		return await query.OrderByDescending(x => x.CreatedAt).ToPagedListAsync(page, eachPage);
	}

	public async Task<PagedList<BaseUser>> GetAllModerator(int page, int eachPage, string? search, IEnumerable<string>? status)
	{
		var query = _entities
			.Include(r => r.Role)
			.Where(x => !x.RoleId!.Equals((int)RoleEnum.Admin) && x.RoleId.Equals((int)RoleEnum.Moderator))
			.AsSplitQuery();

		if (!string.IsNullOrEmpty(search))
		{
			query = query.Where(x => x.Username!.Contains(search) || x.Email!.Contains(search) || x.Fullname!.Contains(search));
		}

        if (status.Any())
        {
            query = query.Where(x => status!.Contains(x.Status));
        }

        return await query.OrderByDescending(x => x.CreatedAt).ToPagedListAsync(page, eachPage);
	}

	public async Task<IEnumerable<BaseUser>> GetAll(IEnumerable<string> userIds)
	{
		return await _entities
			.Include(e => e.Role)
			.Where(x => userIds.Contains(x.Id.ToString())).ToListAsync();
    }

	public async Task<IEnumerable<BaseUser>> GetAllByEmails(IEnumerable<string> emails)
    {
        return await _entities
            .Include(e => e.Role)
            .Where(x => emails.Contains(x.Email!)).ToListAsync();
    }
}