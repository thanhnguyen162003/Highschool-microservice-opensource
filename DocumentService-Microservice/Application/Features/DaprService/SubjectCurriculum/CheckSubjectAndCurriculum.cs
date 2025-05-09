using Application.Common.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DaprService.SubjectCurriculum
{
	public record CheckSubjectAndCurriculum : IRequest<ResponseModel>
	{
		public required Guid SubjectId { get; init; }
		public required Guid CurriculumId { get; init; }
	}
	public class CheckSubjectAndCurriculumHandler(IUnitOfWork unitOfWork)
	: IRequestHandler<CheckSubjectAndCurriculum, ResponseModel>
	{
		public async Task<ResponseModel> Handle(CheckSubjectAndCurriculum request, CancellationToken cancellationToken)
		{
			var subject = await unitOfWork.SubjectRepository.GetSubjectBySubjectId(request.SubjectId);
			if (subject == null)
			{
				return new ResponseModel(System.Net.HttpStatusCode.NotFound, "Cant find subject");
			}
			var curriculum = await unitOfWork.CurriculumRepository.GetCurriculumById(request.CurriculumId);
			if (curriculum == null)
			{
				return new ResponseModel(System.Net.HttpStatusCode.NotFound, "Cant find curriculum");
			}
			var subjectCurriculum = await unitOfWork.SubjectCurriculumRepository.
				GetSubjectCurriculum(request.SubjectId, request.CurriculumId);
			if (subjectCurriculum == null)
			{
				return new ResponseModel(System.Net.HttpStatusCode.NotFound, "Cant find subjectCurriculum");
			}
			return new ResponseModel(System.Net.HttpStatusCode.Found, "Success", subjectCurriculum.Id);
		}
	}
}
