//using Grpc.Core;
//using Infrastructure.Repositories.Interfaces;
//using System.Diagnostics;

//namespace Application.Services.ExternalService
//{
//	public class SubjectService(IUnitOfWork unitOfWork) : SubjectServiceRpc.SubjectServiceRpcBase
//	{
//		public override async Task<SubjectNameResponse> CheckSubjectName(SubjectNameRequest request, ServerCallContext context)
//		{
//			var subjects = await unitOfWork.SubjectRepository.CheckSubjectName(request.SubjectName);

//			var response = new SubjectNameResponse();

//			response.SubjectName.AddRange(subjects);

//			return response;
//		}
//		public override async Task<SubjectGradeResponse> GetSubjectGrade(SubjectGradeRequest request, ServerCallContext context)
//		{
//			Dictionary<string, string> test = new Dictionary<string, string>();
			
//				test = await unitOfWork.SubjectRepository.GetGrade(request.SubjectId.ToList());
			

//			var response = new SubjectGradeResponse();

//			response.SubjectId.AddRange(test.Keys);
//            response.Grade.AddRange(test.Values);

//            return response;
//		}
//        public override async Task<SubjectEnrollCheckResponse> GetSubjectEnroll(SubjectEnrollCheckRequest request, ServerCallContext context)
//        {
//            Dictionary<string, string> test = new Dictionary<string, string>();
           
//				var subjectCurriculumId = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculumIdBySubjectId(request.SubjectId);
//                var check = await unitOfWork.EnrollmentRepository.GetSubjectAndUserEnroll(request.UserId, subjectCurriculumId);
//			foreach (var key in check) 
//			{
//				var result = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculumById(Guid.Parse(key.Key));
//				test.Add(result.SubjectId.ToString(), key.Value);
//			}

//            var response = new SubjectEnrollCheckResponse();

//            response.SubjectId.AddRange(test.Keys);
//            response.UserId.AddRange(test.Values);

//            return response;
//        }
//    }
//}
