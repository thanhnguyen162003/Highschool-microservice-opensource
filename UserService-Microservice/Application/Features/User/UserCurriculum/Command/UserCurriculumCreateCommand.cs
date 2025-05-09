using Application.Common.Models.Common;
using Dapr.Client;
using Domain.Common.UUID;
using Domain.Entities;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.User.UserCurriculum.Command
{
	public class UserCurriculumCreateCommand : IRequest<ResponseModel>
	{
		public required Guid SubjectId { get; set; }
		public required Guid CurriculumId { get; set; }
	}
	public class UserCurriculumCreateCommandHandler(
	ILogger<UserCurriculumCreateCommandHandler> logger,
	DaprClient client,
	IUnitOfWork unitOfWork,
	IAuthenticationService authenticationService)
	: IRequestHandler<UserCurriculumCreateCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(UserCurriculumCreateCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var userId = authenticationService.GetUserId();
				var response = await client.InvokeMethodAsync<ResponseModel>(
							HttpMethod.Get,
							"document-sidecar",
							$"api/v1/dapr/check-subject-curriculum-id/subject/{request.SubjectId}/curriculum/{request.CurriculumId}"
						);
				if (response.Status == HttpStatusCode.NotFound)
				{
					return new ResponseModel(response.Status, response.Message);	
				}
				var checkExits = await unitOfWork.ChosenSubjectCurriculumRepository
					.GetChosenByUserIdAndSubjectId(userId, request.SubjectId, cancellationToken);
				if(checkExits is not null)
				{
					checkExits.CurriculumId = request.CurriculumId;
					checkExits.SubjectCurriculumId = Guid.Parse(response.Data.ToString());
					checkExits.UpdatedAt = DateTime.Now;
					var resultUpdate = await unitOfWork.ChosenSubjectCurriculumRepository.UpdateChosenSubjectCurriculum(checkExits);
					if(resultUpdate is false)
					{
						return new ResponseModel(HttpStatusCode.BadRequest, "Cập nhật lỗi");
					}
					return new ResponseModel(HttpStatusCode.OK, "Cập nhật thành công");
				}
				ChosenSubjectCurriculum chosenSubjectCurriculum = new ChosenSubjectCurriculum()
				{
					Id = new UuidV7().Value,
					UserId = userId,
					SubjectId = request.SubjectId,
					CurriculumId = request.CurriculumId,
					SubjectCurriculumId = Guid.Parse(response.Data.ToString())
				};
				var result = await unitOfWork.ChosenSubjectCurriculumRepository.InsertChosenSubjectCurriculum(chosenSubjectCurriculum, cancellationToken);
				if(result is false)
				{
					return new ResponseModel(HttpStatusCode.BadRequest, "Insert Db lỗi");
				}
				return new ResponseModel(HttpStatusCode.OK, "Tạo mới thành công");
			}
			catch (Exception e)
			{
				logger.LogError(e, "Fail in Catch Exception");
				return new ResponseModel(HttpStatusCode.BadRequest, "Fail in Catch Exception");
			}
		}
	}
}
