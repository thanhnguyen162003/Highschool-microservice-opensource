using Application.Common.Models.Common;
using Application.Constants;
using Application.Service.Cloudinary;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Repositories.Interfaces;
using SharedProject.Models;
using System.Net;

namespace Application.Features.ReportApp.CreateReport
{
    public class CreateReportCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,
        ICloudinaryService cloudinaryService, IProducerService producerService) : IRequestHandler<CreateReportCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            // Create Report
            var reportId = Guid.NewGuid();
            var report = new Report
            {
                Id = reportId,
                ReportTitle = request.ReportTitle,
                ReportContent = request.ReportContent,
                Status = ReportStatus.New.ToString(),
                UserId = _httpContextAccessor.HttpContext!.User.GetUserIdFromToken(),
                CreatedAt = DateTime.Now,
            };

            await _unitOfWork.ReportRepository.AddAsync(report);

            // Create Report Image
            if (request.Images != null)
            {
                await UploadImageReport(reportId, request.Images);
            }

            if (await _unitOfWork.SaveChangesAsync())
            {
                NotificationUserModel dataModel = new NotificationUserModel()
                {
                    UserId = _httpContextAccessor.HttpContext!.User.GetUserIdFromToken().ToString(),
                    Content = "Cảm ơn bạn đã report góp phần xây dựng cộng đồng học tập tốt hơn!",
                    Title = "Report đã được gửi cho Admin!",
                };

                _ = Task.Run(async () =>
                {
                    var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.NotificationUserCreated, _httpContextAccessor.HttpContext!.User.GetUserIdFromToken().ToString(), dataModel);
                }, cancellationToken);
                return new ResponseModel
                {
                    Status = HttpStatusCode.Created,
                    Message = MessageCommon.CreateSuccesfully
                };
            }

            return new ResponseModel
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.CreateFailed
            };
        }

        private async Task UploadImageReport(Guid reportId, IEnumerable<IFormFile> images)
        {
            foreach (var image in images)
            {
                var extension = Path.GetExtension(image.FileName);
                var imageId = Guid.NewGuid();
                var nameImage = imageId.ToString() + "@" + DateTime.Now.Ticks + "." + extension;
                var responseImage = await _cloudinaryService.UploadAsync(image, nameImage);
                if (responseImage.Status != HttpStatusCode.OK)
                {
                    return;
                }

                await _unitOfWork.ImageReportRepository.AddAsync(new ImageReport
                {
                    ReportId = reportId,
                    ImageUrl = responseImage.Url
                });
            }
        }
    }
}
