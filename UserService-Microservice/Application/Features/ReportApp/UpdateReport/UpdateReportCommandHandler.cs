using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Enumerations;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.ReportApp.UpdateReport
{
    public class UpdateReportCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateReportCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResponseModel> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
        {
            var reportStatus = request.Status.ConvertToValue<ReportStatus>();

            if (reportStatus == null)
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageCommon.InvalidStatus
                };
            }

            var report = await _unitOfWork.ReportRepository.GetByIdAsync(request.ReportId);

            if (report == null)
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound
                };
            }

            report.Status = reportStatus.Value.ToString();
            _unitOfWork.ReportRepository.Update(report);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.UpdateSuccesfully
                };
            }

            return new ResponseModel
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.UpdateFailed
            };
        }
    }
}
