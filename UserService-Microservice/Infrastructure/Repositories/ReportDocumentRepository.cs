using Domain.Common.Models;
using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Extensions;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
	public class ReportDocumentRepository(UserDatabaseContext context) : BaseRepository<ReportDocument>(context), IReportDocumentRepository
	{
		private readonly UserDatabaseContext _context = context;

        public async Task<int> GetTotalReportCount()
        {
			return await _entities.CountAsync();
        }
        public async Task<int> GetReportLastMonth()
        {
            var test = DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month);
            return await _entities.CountAsync(x => x.CreatedAt >= new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1, 0, 0, 0) && x.CreatedAt <= new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, test, 23, 59, 59));
        }
        public async Task<int> GetReportThisMonth()
        {
            return await _entities.CountAsync(x => x.CreatedAt >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0));
        }
        public async Task<int> GetTotalFlashcardReport()
        {
            return await _entities.CountAsync(x => x.ReportType == ReportType.Flashcard);
        }
        public async Task<int> GetTotalFlashcardContentReport()
        {
            return await _entities.CountAsync(x => x.ReportType == ReportType.FlashcardContent);
        }
        public async Task<int> GetTotalCommentReport()
        {
            return await _entities.CountAsync(x => x.ReportType == ReportType.Comment);
        }
        public async Task<int> GetTotalSubjectReport()
        {
            return await _entities.CountAsync(x => x.ReportType == ReportType.Subject);
        }
        public async Task<int> GetTotalChapterReport()
        {
            return await _entities.CountAsync(x => x.ReportType == ReportType.Chapter);
        }
        public async Task<int> GetTotalLessonReport()
        {
            return await _entities.CountAsync(x => x.ReportType == ReportType.Lesson);
        }
        public async Task<int> GetTotalDocumentReport()
        {
            return await _entities.CountAsync(x => x.ReportType == ReportType.Document);
        }
        public async Task<int> GetTotalQuizReport()
        {
            return await _entities.CountAsync(x => x.ReportType == ReportType.Quiz);
        }
        public async Task<int> GetTotalTestReport()
        {
            return await _entities.CountAsync(x => x.ReportType == ReportType.Test);
        }
        public async Task<bool> CreateReportDocument(ReportDocument reportDocument, CancellationToken cancellationToken)
		{
			var data = _context.ReportDocuments.Add(reportDocument);
			var result = await _context.SaveChangesAsync(cancellationToken);
			return result > 0;
		}

		public async Task<PagedList<ReportDocument>> GetReportDocumentForAdmin(int page, int eachPage, CancellationToken cancellationToken)
		{
			var query = _entities
			.AsNoTracking()
			.Include(e => e.User).ThenInclude(e => e.Role)
			.AsSplitQuery();

			return await query.OrderByDescending(x => x.CreatedAt).ToPagedListAsync(page, eachPage);
		}

		public async Task<PagedList<ReportDocument>> GetReportDocumentForUser(Guid userId, int page, int eachPage, CancellationToken cancellationToken)
		{
			var query = _context.ReportDocuments
				.Where(rd => rd.UserId.Equals(userId))
				.AsNoTracking()
				.Include(u => u.User).ThenInclude(e => e.Role)
				.AsSplitQuery();

			return await query.OrderByDescending(x => x.CreatedAt).ToPagedListAsync(page, eachPage);
		}

		public async Task<bool> IsReportDocumentExits(Guid userId, Guid documentId, ReportType reportType, CancellationToken cancellationToken)
		{
			var reportDocument = await _context.ReportDocuments
				.FirstOrDefaultAsync(rd => rd.UserId.Equals(userId)
				&& rd.DocumentId.Equals(documentId)
				&& rd.ReportType.Equals(reportType)
				&& !rd.Status.Equals(ReportStatus.Closed), cancellationToken);
			return reportDocument != null;
		}
	}
}
