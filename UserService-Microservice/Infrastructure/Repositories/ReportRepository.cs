using Domain.Common.Models;
using Infrastructure.Extensions;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
	public class ReportRepository(UserDatabaseContext context) : BaseRepository<Report>(context), IReportRepository
	{
		private readonly UserDatabaseContext _context = context;

        public async Task<int> GetTotalReportAmount()
        {
            return await _entities.CountAsync();
        }
        public async Task<int> GetTotalReportThisMonth()
        {
            return await _entities.CountAsync(x => x.CreatedAt >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0));
        }
        public async Task<int> GetTotalReportLastMonth()
        {
			var test = DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month);
            return await _entities.CountAsync(x => x.CreatedAt >= new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1, 0, 0, 0) && x.CreatedAt <= new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, test, 23, 59, 59));
        }
        public async Task<PagedList<Report>> GetReport(int page, int eachPage, string? status,
												DateTime? startDate, DateTime? endDate, Guid? reportId,
												bool isAscending = false)
		{
			var reports = _context.Reports
				.Include(r => r.ImageReports)
				.Include(r => r.User).ThenInclude(u => u.Role)
				.AsQueryable()
				.AsNoTracking()
				.AsSplitQuery();

			if (status != null)
			{
				reports = reports.Where(r => r.Status.Equals(status));
			}

			if (startDate != null)
			{
				reports = reports.Where(r => startDate > r.CreatedAt);
			}

			if (endDate != null)
			{
				reports = reports.Where(r => endDate < r.CreatedAt);
			}

            if (reportId != null)
            {
                reports = reports.Where(r => r.Id.Equals(reportId));
            }

            return await reports.ToPaginateAndSort(page, eachPage, "CreatedAt", isAscending);
		}

		public async Task<PagedList<Report>> GetReport(int page, int eachPage, Guid? userId, string? status,
												DateTime? startDate, DateTime? endDate, Guid? reportId,
                                                bool isAscending = false)
		{
			var reports = _context.Reports
				.Include(r => r.ImageReports)
				.Include(r => r.User).ThenInclude(u => u.Role)
				.Where(r => r.UserId.Equals(userId))
				.AsQueryable()
				.AsNoTracking()
				.AsSplitQuery();

			if (status != null)
			{
				reports = reports.Where(r => r.Status.Equals(status));
			}

			if (startDate != null)
			{
				reports = reports.Where(r => startDate > r.CreatedAt);
			}

			if (endDate != null)
			{
				reports = reports.Where(r => endDate < r.CreatedAt);
			}

			if(reportId != null || reportId != Guid.Empty)
            {
                reports = reports.Where(r => r.Id.Equals(reportId));
            }

            return await reports.ToPaginateAndSort(page, eachPage, "CreatedAt", isAscending);
		}



	}
}
