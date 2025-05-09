using Application.Common.Models.Common;
using Application.Common.Models.ReportDocumentModel;
using Application.Constants;
using Dapr.Client;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Common.UUID;
using Domain.DaprModels;
using Domain.Enumerations;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.ReportDocument.Commands
{
	public record CreateReportDocumentCommand : IRequest<ResponseModel>
	{
		public required CreateReportDocumentRequestModel CreateReportDocumentRequestModel { get; init; }
	}
	public class CreateReportDocumentCommandHandler(
		IUnitOfWork unitOfWork,
		IHttpContextAccessor httpContextAccessor,
		IProducerService produceService,
		DaprClient client)
		: IRequestHandler<CreateReportDocumentCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(CreateReportDocumentCommand request, CancellationToken cancellationToken)
		{
			var isReportExits = await unitOfWork.ReportDocumentRepository.IsReportDocumentExits(httpContextAccessor.HttpContext!.User.GetUserIdFromToken(),
				request.CreateReportDocumentRequestModel.DocumentId, request.CreateReportDocumentRequestModel.ReportType, cancellationToken);

			if (isReportExits)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.BadRequest,
					Message = "Bạn chỉ có thể report 1 lần cho mỗi tài nguyên"
				};
			}
			var daprRequest = new CheckResourceExistsRequest
			{
				ResourceId = request.CreateReportDocumentRequestModel.DocumentId.ToString(),
				ResourceType = request.CreateReportDocumentRequestModel.ReportType.ToString()
			};

			var daprResponse = await client.InvokeMethodAsync<CheckResourceExistsResponseDapr>(
				HttpMethod.Get,
				"document-sidecar",
				$"api/v1/dapr/check-resources?resourceId={Uri.EscapeDataString(daprRequest.ResourceId)}&resourceType={Uri.EscapeDataString(daprRequest.ResourceType)}",
				cancellationToken
			);

			if (daprResponse.Result == null || !daprResponse.Result.Exists)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.NotFound,
					Message = "Tài nguyên không tồn tại"
				};
			}

			if (daprResponse.Result.ResourceType != request.CreateReportDocumentRequestModel.ReportType.ToString())
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.BadRequest,
					Message = $"Tài nguyên có ID {request.CreateReportDocumentRequestModel.DocumentId.ToString()} không phải là {request.CreateReportDocumentRequestModel.ReportType}"
				};
			}

			var report = new Domain.Entities.ReportDocument
			{
				Id = new UuidV7().Value,
				ReportTitle = request.CreateReportDocumentRequestModel.ReportTitle,
				ReportContent = request.CreateReportDocumentRequestModel.ReportContent,
				Status = ReportStatus.New.ToString(),
				DocumentId = request.CreateReportDocumentRequestModel.DocumentId,
				ReportType = request.CreateReportDocumentRequestModel.ReportType,
				UserId = httpContextAccessor.HttpContext!.User.GetUserIdFromToken(),
				CreatedAt = DateTime.Now,
			};

			var result = await unitOfWork.ReportDocumentRepository.CreateReportDocument(report, cancellationToken);
			
			if (result is true)
			{
				await produceService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.ReportDocumentCreated,
					httpContextAccessor.HttpContext!.User.GetUserIdFromToken().ToString(), report);
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

	}
}
