using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardContentModel;
using Application.Features.FlashcardContentFeature.Queries;
using Domain.CustomEntities;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.EnrollmentFeature.Queries
{
    public class TopEnrollmentQuery : IRequest<List<TopEnrolledSubjectModel>>
    {
        public string Filter { get; set; }
    }
    public class TopEnrollmentQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions,
    IClaimInterface claim)
    : IRequestHandler<TopEnrollmentQuery, List<TopEnrolledSubjectModel>>
    {
        public async Task<List<TopEnrolledSubjectModel>> Handle(TopEnrollmentQuery request, CancellationToken cancellationToken)
        {
            if (request.Filter.ToLower() == "course") 
            {
                return await unitOfWork.EnrollmentRepository.GetEnrollmentCompletionStatus();
            }
            else if (request.Filter.ToLower() == "flashcard")
            {

                return await unitOfWork.UserFlashcardProgressRepository.GetEnrollmentCompletionStatus();
            }
            return new List<TopEnrolledSubjectModel>();


        }
    }
}
