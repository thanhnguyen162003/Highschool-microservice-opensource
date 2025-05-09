using Application.Common.Models.ReportModel;
using Domain.Common.Models;

namespace Application.Features.User.GetReport
{
	public class GetReportCommand : IRequest<PagedList<ReportResponseModel>>
	{
		public int Page { get; set; }
		public int EachPage { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string? Status { get; set; }
		public bool IsAscending { get; set; } = false;
		public Guid? ReportId { get; set; }
    }
}
