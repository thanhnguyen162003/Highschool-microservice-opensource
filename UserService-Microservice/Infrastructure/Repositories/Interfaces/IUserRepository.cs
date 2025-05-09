using Domain.Common.Models;

namespace Infrastructure.Repositories.Interfaces;

public interface IUserRepository : IRepository<BaseUser>
{
	Task<Dictionary<string, int>> CountUserByRole(IEnumerable<string> userIds);
    Task<int> GetTotalUserAmount();
	Task<int> GetUserAmountCreateThisMonth();
	Task<int> GetUserAmountCreateLastMonth();
    Task<int> GetTotalActiveUserAmount();
	Task<int> GetTotalBlockedUserAmount();
    Task<int> GetTotalDeletedUserAmount();
    Task<BaseUser?> GetUserByUsernameOrEmail(string username);
	Task<BaseUser?> GetUserByUserId(Guid userId);
	Task<bool> IsExistEmail(string email);
	Task<bool> IsExistUserName(string username, Guid userId);
	Task<int?> GetRoleId(string username);
	Task<BaseUser?> GetDetailUser(Guid userId);
	Task<List<BaseUser>> GetUserForMedia();

    Task<BaseUser?> GetUserByUsername(string username);
	Task<PagedList<BaseUser>> GetAllUser(int page, int eachPage, int roleId, string? search, IEnumerable<string>? status);
	Task<PagedList<BaseUser>> GetAllModerator(int page, int eachPage, string? search, IEnumerable<string>? status);
	Task<IEnumerable<BaseUser>> GetAll(IEnumerable<string> userIds);
	Task<BaseUser?> GetUser(string key);
	Task<IEnumerable<BaseUser>> GetAllByEmails(IEnumerable<string> emails);
}