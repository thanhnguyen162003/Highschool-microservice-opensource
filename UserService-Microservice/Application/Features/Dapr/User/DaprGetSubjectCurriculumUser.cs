using Application.Common.Models.Common;
using Application.Features.Dapr.Users;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.Dapr.User
{
	public class DaprGetSubjectCurriculumUser : IRequest<ResponseModel>
	{
		public Guid UserId { get; set; }
		public Guid SubjectId { get; set; }
	}
	public class DaprGetSubjectCurriculumUserHandler(IUnitOfWork unitOfWork)
	: IRequestHandler<DaprGetSubjectCurriculumUser, ResponseModel>
	{
		public async Task<ResponseModel> Handle(DaprGetSubjectCurriculumUser request, CancellationToken cancellationToken)
		{
			var chosenSubjectCurriculum = await unitOfWork.ChosenSubjectCurriculumRepository
				.GetChosenByUserIdAndSubjectId(request.UserId, request.SubjectId, cancellationToken);
			if (chosenSubjectCurriculum == null)
			{
				return new ResponseModel(System.Net.HttpStatusCode.NotFound, "No chosse yet");
			}
			return new ResponseModel(System.Net.HttpStatusCode.Found, "Founded", chosenSubjectCurriculum.CurriculumId);
		}
	}
}
