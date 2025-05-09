using Application.Common.Models.Common;

namespace Application.Features.ReportApp.UpdateReport
{
    public class UpdateReportCommand : IRequest<ResponseModel>
    {
        public Guid ReportId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
