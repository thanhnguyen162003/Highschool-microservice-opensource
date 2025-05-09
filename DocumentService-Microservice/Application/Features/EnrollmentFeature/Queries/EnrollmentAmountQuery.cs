using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.DaprModel;
using Dapr.Client;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.EnrollmentFeature.Queries
{
    public class EnrollmentAmountQuery : IRequest<List<EnrollmentAmountResponse>>
    {
    }
    public class EnrollmentAmountQueryHandler(
    DaprClient dapr,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claim)
    : IRequestHandler<EnrollmentAmountQuery, List<EnrollmentAmountResponse>>
    {
        public async Task<List<EnrollmentAmountResponse>> Handle(EnrollmentAmountQuery request, CancellationToken cancellationToken)
        {
            var sc = await unitOfWork.EnrollmentRepository.GetEnrollAmount();
            var flashcard = await unitOfWork.UserFlashcardProgressRepository.GetEnrollAmount();

            //dapr
            var zoneDapr = await dapr.InvokeMethodAsync<ZoneResponseDapr>(
                    HttpMethod.Get,
                    "academy-sidecar",
                    $"api/v1/dapr/zone-member",
                    cancellationToken
                );

            var result = new List<EnrollmentAmountResponse>();
            result.Add(new EnrollmentAmountResponse
            {
                Name = "Course",
                Count = sc
            });
            result.Add(new EnrollmentAmountResponse
            {
                Name = "Flashcard",
                Count = flashcard
            });
            result.Add(new EnrollmentAmountResponse
            {
                Name = "Zone",
                Count = zoneDapr.TotalZoneMember
            });

            return result;
        }
    }
}
