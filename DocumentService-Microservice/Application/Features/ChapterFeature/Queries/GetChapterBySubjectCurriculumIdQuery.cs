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

public class GetChapterBySubjectCurriculumIdQuery : IRequest<PagedList<ChapterResponseModel>>
{
    public Guid SubjectCurriculumId;
    public required ChapterQueryFilter QueryFilter;
}

public class GetChapterBySubjectCurriculumIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions,
    IProducerService producerService, IClaimInterface claimService, ILogger<GetChapterBySubjectCurriculumIdQueryHandler> logger) : IRequestHandler<GetChapterBySubjectCurriculumIdQuery, PagedList<ChapterResponseModel>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly PaginationOptions _paginationOptions;
    private readonly ILogger<GetChapterBySubjectCurriculumIdQueryHandler> _logger = logger;
    private readonly IProducerService _producerService = producerService;
    private readonly IClaimInterface _claimService = claimService;

    public async Task<PagedList<ChapterResponseModel>> Handle(GetChapterBySubjectCurriculumIdQuery request, CancellationToken cancellationToken)
    {
        var userRole = _claimService.GetRole;
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;
        var (listChapter, totalCount) = (new List<Chapter>(), default(int));
        if (!string.IsNullOrWhiteSpace(userRole) && userRole.Equals("Moderator"))
        {
            (listChapter, totalCount) = await _unitOfWork.ChapterRepository.GetChaptersBySubjectCurriculumIdModerator(request.QueryFilter,request.SubjectCurriculumId);
        }
        else
        {
            (listChapter, totalCount) = await _unitOfWork.ChapterRepository.GetChaptersBySubjectCurriculumId(request.QueryFilter,request.SubjectCurriculumId);
        }
        if (!listChapter.Any())
        {
            return new PagedList<ChapterResponseModel>(new List<ChapterResponseModel>(), 0, 0, 0);
        }
        var mapperList = _mapper.Map<List<ChapterResponseModel>>(listChapter);
        if (_claimService.GetCurrentUserId != Guid.Empty)
        {
            // UserAnalyseMessageModel dataModel = new UserAnalyseMessageModel()
            // {
            //     UserId = _claimService.GetCurrentUserId,
            //     SubjectId = listChapter.First().SubjectCurriculum.SubjectId,
            // };
            //
            // _ = Task.Run(() =>
            // {
            //    _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.UserAnalyseData, _claimService.GetCurrentUserId.ToString(), dataModel);
            // }, cancellationToken);
            
            if (_claimService.GetRole.Contains(RoleConstaint.STUDENT.ToString(), StringComparison.Ordinal))
            {
                var enroll = await _unitOfWork.EnrollmentRepository.Get(enroll => enroll.SubjectCurriculumId == request.SubjectCurriculumId && enroll.BaseUserId == _claimService.GetCurrentUserId);
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