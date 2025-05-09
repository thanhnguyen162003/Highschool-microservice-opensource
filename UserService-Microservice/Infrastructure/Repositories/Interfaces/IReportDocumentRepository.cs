using Domain.Common.Models;
using Domain.Enumerations;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IReportDocumentRepository : IRepository<ReportDocument>
	{
		Task<int> GetTotalReportCount();
		Task<int> GetReportLastMonth();
		Task<int> GetReportThisMonth();
		Task<int> GetTotalFlashcardReport();
        Task<int> GetTotalFlashcardContentReport();
        Task<int> GetTotalCommentReport();
        Task<int> GetTotalSubjectReport();
        Task<int> GetTotalChapterReport();
        Task<int> GetTotalLessonReport();
        Task<int> GetTotalDocumentReport();
        Task<int> GetTotalQuizReport();
        Task<int> GetTotalTestReport();
        Task<bool> IsReportDocumentExits(Guid userId, Guid documentId, ReportType reportType, CancellationToken cancellationToken);
		Task<bool> CreateReportDocument(ReportDocument reportDocument, CancellationToken cancellationToken);
		Task<PagedList<ReportDocument>> GetReportDocumentForUser(Guid userId, int page, int eachPage, CancellationToken cancellationToken);
		Task<PagedList<ReportDocument>> GetReportDocumentForAdmin(int page, int eachPage, CancellationToken cancellationToken);
	}
}
