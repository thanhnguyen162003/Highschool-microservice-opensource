using Application.Common.Models.FlashcardModel;
using Application.Common.Models.SubjectCurriculumModel;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.StatisticFeature.Queries
{
    public class SubjectCurriculumCountQuery : IRequest<SubjectCurriculumCountResponseModel>
    {
    }
    public class SubjectCurriculumCountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<SubjectCurriculumCountQuery, SubjectCurriculumCountResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<SubjectCurriculumCountResponseModel> Handle(SubjectCurriculumCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculumCount(cancellationToken);
            var result = new SubjectCurriculumCountResponseModel
            {
                Published = count.TryGetValue("Publish", out var publishCount) ? publishCount : 0,
                UnPublished = count.TryGetValue("UnPublish", out var unpublishCount) ? unpublishCount : 0
            };

            return result;
        }
    }
}
