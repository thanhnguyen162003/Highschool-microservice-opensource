using Application.Common.Models;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.SubjectCurriculumFeature.Queries
{
	public class SubjectCurriculumCheckQuery : IRequest<ResponseModel>
	{
		public Guid SubjectId;
		public Guid CurriculumId;
	}
	public class SubjectCurriculumCheckQueryHandler(
		IUnitOfWork unitOfWork)
		: IRequestHandler<SubjectCurriculumCheckQuery, ResponseModel>
	{
		public async Task<ResponseModel> Handle(SubjectCurriculumCheckQuery request, CancellationToken cancellationToken)
		{
			var result = await unitOfWork.SubjectCurriculumRepository.IsSubjectCurriculumExists(request.SubjectId, request.CurriculumId, cancellationToken);
			if (result == false)
			{
				return new ResponseModel(HttpStatusCode.Found, "Subject Curriculum is not published", false);
			}
			return new ResponseModel(HttpStatusCode.Found, "Subject Curriculum is already published", true);
		}
	}
}
