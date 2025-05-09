using Application.Common.Models.DaprModel.Enrollment;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DaprService.Enrollment
{
    public record DaprGetEnrollment : IRequest<EnrollmentResponseDapr>
    {
    }
    public class DaprGetEnrollmentHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetEnrollment, EnrollmentResponseDapr>
    {
        public async Task<EnrollmentResponseDapr> Handle(DaprGetEnrollment request, CancellationToken cancellationToken)
        {
            var list = await unitOfWork.EnrollmentRepository.GetEnrollmentCount();
            var enrollmentResponse = new EnrollmentResponseDapr();

            foreach (var item in list)
            {
                if (item.LessonLearnDate.Count == 0)
                {
                    continue; // Skip items with no LessonLearnDate
                }

                var enrollmentObject = new EnrollmentObjectDapr
                {
                    UserId = item.UserId.ToString(),
                    LessonLearnDate = item.LessonLearnDate.Select(x => x.ToString("o")).ToList(),
                };

                enrollmentResponse.Enrollment.Add(enrollmentObject);
            }


            return enrollmentResponse;
        }
    }
}
