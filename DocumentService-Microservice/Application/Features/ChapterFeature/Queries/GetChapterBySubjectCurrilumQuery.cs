using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.ChapterModel;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.ChapterFeature.Queries;

public record GetChapterBySubjectCurrilumQuery : IRequest<PagedList<ChapterResponseModel>>
{
    public Guid SubjectId;
    public Guid CurriculumId;
    public ChapterQueryFilter QueryFilter;
}

public class GetChapterBySubjectCurrilumQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions,
    IProducerService producerService, IClaimInterface claimService, ILogger<GetChapterBySubjectCurrilumQueryHandler> logger) : IRequestHandler<GetChapterBySubjectCurrilumQuery, PagedList<ChapterResponseModel>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly PaginationOptions _paginationOptions;
    private readonly ILogger<GetChapterBySubjectCurrilumQueryHandler> _logger = logger;
    private readonly IProducerService _producerService = producerService;
    private readonly IClaimInterface _claimService = claimService;

    public async Task<PagedList<ChapterResponseModel>> Handle(GetChapterBySubjectCurrilumQuery request, CancellationToken cancellationToken)
    {
        var userRole = _claimService.GetRole;
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;
        var (listChapter, totalCount) = (new List<Chapter>(), default(int));
        if (!string.IsNullOrWhiteSpace(userRole) && userRole.Equals("Moderator"))
        {
            (listChapter, totalCount) = await _unitOfWork.ChapterRepository.GetChaptersBySubjectCurriculumModerator(request.QueryFilter,request.CurriculumId, request.SubjectId);
        }
        else
        {
            (listChapter, totalCount) = await _unitOfWork.ChapterRepository.GetChaptersBySubjectCurriculum(request.QueryFilter,request.CurriculumId, request.SubjectId);
        }
        if (!listChapter.Any())
        {
            return new PagedList<ChapterResponseModel>(new List<ChapterResponseModel>(), 0, 0, 0);
        }
        var mapperList = _mapper.Map<List<ChapterResponseModel>>(listChapter);
        if (_claimService.GetCurrentUserId != Guid.Empty)
        {
            if (_claimService.GetRole.Contains(RoleConstaint.STUDENT.ToString(), StringComparison.Ordinal))
            {
                var enroll = await _unitOfWork.EnrollmentRepository.Get(enroll => enroll.SubjectCurriculum.SubjectId == request.SubjectId && enroll.SubjectCurriculum.CurriculumId == request.CurriculumId && enroll.BaseUserId == _claimService.GetCurrentUserId);
                if (enroll.FirstOrDefault() != null)
                {
                    for (int i = 0; i < mapperList.Count(); i++)
                    {
                        var chapterResponse = mapperList[i];
                        var lessonList = await _unitOfWork.LessonRepository.Get(lesson => lesson.ChapterId == chapterResponse.Id);

                        var progressList = await _unitOfWork.EnrollmentProgressRepository.Get(progress =>
                                            progress.Lesson.Chapter.Id == chapterResponse.Id &&
                                            progress.Enrollment.BaseUserId == _claimService.GetCurrentUserId);

                        chapterResponse.IsDone = !lessonList.Any() ? true : (lessonList.Count() == progressList.Count());
                    }
                }
            }
        }     
        return new PagedList<ChapterResponseModel>(mapperList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }
}