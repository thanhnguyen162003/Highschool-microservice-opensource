//using Grpc.Core;
//using Infrastructure.Repositories.Interfaces;
//using System.Diagnostics;

//namespace Application.Services.ExternalService
//{
//	public class EnrollmentService(IUnitOfWork unitOfWork) : EnrollmentServiceRpc.EnrollmentServiceRpcBase
//	{
//        public override async Task<EnrollmentResponse> GetEnrollment(EnrollmentRequest request, ServerCallContext context)
//        {
//            var list = await unitOfWork.EnrollmentRepository.GetEnrollmentCount();
//            var enrollmentResponse = new EnrollmentResponse();

//            foreach (var item in list)
//            {
//                if (item.LessonLearnDate.Count == 0)
//                {
//                    continue; // Skip items with no LessonLearnDate
//                }

//                var enrollmentObject = new EnrollmentObject
//                {
//                    UserId = item.UserId.ToString()
//                };

//                foreach (var date in item.LessonLearnDate)
//                {
//                    enrollmentObject.LessonLearnDate.Add(date.ToString("o"));
//                }

//                enrollmentResponse.Enrollment.Add(enrollmentObject);
//            }


//            return enrollmentResponse;
//        }
//    }
//}
