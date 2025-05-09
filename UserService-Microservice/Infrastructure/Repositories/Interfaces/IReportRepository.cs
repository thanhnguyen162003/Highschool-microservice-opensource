using Domain.Common.Models;

namespace Infrastructure.Repositories.Interfaces
{
	public interface IReportRepository : IRepository<Report>
	{
		Task<int> GetTotalReportAmount();
		Task<int> GetTotalReportThisMonth();
		Task<int> GetTotalReportLastMonth();
        Task<PagedList<Report>> GetReport(int page, int eachPage, string? status, DateTime? startDate, DateTime? endDate, Guid? reportId, bool isAscending = false);
		Task<PagedList<Report>> GetReport(int page, int eachPage, Guid? userId, string? status,
													DateTime? startDate, DateTime? endDate, Guid? reportId,
                                                    bool isAscending = false);
	}
}
