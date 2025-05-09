using Application.Common.Models.Common;

namespace Application.Features.ReportApp.CreateReport
{
    public class CreateReportCommand : IRequest<ResponseModel>
    {
        public string ReportTitle { get; set; } = null!;

        public string ReportContent { get; set; } = null!;

        public IEnumerable<IFormFile>? Images { get; set; }
    }
}
