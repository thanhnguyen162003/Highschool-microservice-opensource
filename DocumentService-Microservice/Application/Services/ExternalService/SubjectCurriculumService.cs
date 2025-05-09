//using Grpc.Core;
//using Infrastructure.Repositories.Interfaces;
//using System.Diagnostics;

//namespace Application.Services.ExternalService
//{
//	public class SubjectCurriculumService(IUnitOfWork unitOfWork) : SubjectCurriculumServiceRpc.SubjectCurriculumServiceRpcBase
//	{
//		public override async Task<SubjectCurriculumNameResponse> CheckSubjectCurriculumName(SubjectCurriculumNameRequest request, ServerCallContext context)
//		{
//			var subjects = await unitOfWork.SubjectCurriculumRepository.CheckSubjectCurriculumName(request.SubjectCurriculumName);

//			var response = new SubjectCurriculumNameResponse();

//			response.SubjectCurriculumName.AddRange(subjects);

//			return response;
//		}
//    }
//}
