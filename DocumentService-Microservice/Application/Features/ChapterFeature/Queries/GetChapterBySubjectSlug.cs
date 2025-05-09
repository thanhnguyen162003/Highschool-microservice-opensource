using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.ChapterModel;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.ChapterFeature.Queries;

public class GetChapterBySubjectSlug : IRequest<PagedChapterResponseModelWithProgress>
{
    public string SubjectSlug;
    public Guid CurriculumId;
    public ChapterQueryFilter QueryFilter;
}

public class GetChapterBySubjectSlugHandler(IUnitOfWork unitOfWork, IMapper mapper, IProducerService producerService,
    IOptions<PaginationOptions> paginationOptions, IClaimInterface claimInterface) : IRequestHandler<GetChapterBySubjectSlug, PagedChapterResponseModelWithProgress>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly PaginationOptions _paginationOptions;
    private readonly IClaimInterface _claimInterface = claimInterface;
    private readonly IProducerService _producerService = producerService;
    private readonly ILogger<GetChapterBySubjectSlugHandler> _logger;

    public async Task<PagedChapterResponseModelWithProgress> Handle(GetChapterBySubjectSlug request, CancellationToken cancellationToken)
    {
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;
        var subject = await _unitOfWork.SubjectRepository.GetSubjectBySubjectSlug(request.SubjectSlug, cancellationToken);
        if (subject == null)
        {
            return null;
        }
        var (listChapter, totalCount) = await _unitOfWork.ChapterRepository.GetChaptersBySubjectCurriculum(request.QueryFilter, request.CurriculumId, subject.Id);
        if (!listChapter.Any())
        {
            return new PagedChapterResponseModelWithProgress
            {
                Items = new PagedList<ChapterResponseModel>(new List<ChapterResponseModel>(), 0, 0, 0)
            };
        }

        var mapperList = _mapper.Map<List<ChapterResponseModel>>(listChapter);

        if (_claimInterface.GetCurrentUserId != Guid.Empty)
        {
            // UserAnalyseMessageModel dataModel = new UserAnalyseMessageModel()
            // {
            //     UserId = _claimInterface.GetCurrentUserId,
            //     SubjectId = subject.Id
            // };
            //
            // _ = Task.Run(() =>
            // {
            //     _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.UserAnalyseData, _claimInterface.GetCurrentUserId.ToString(), dataModel);
            // }, cancellationToken);
            
            if (_claimInterface.GetRole.Contains(RoleConstaint.STUDENT.ToString(), StringComparison.Ordinal))
            {
                var enrollExist = await _unitOfWork.EnrollmentRepository.Get(enroll => enroll.SubjectCurriculum.Subject.Slug == request.SubjectSlug && enroll.SubjectCurriculum.CurriculumId == request.CurriculumId && enroll.BaseUserId == _claimInterface.GetCurrentUserId);
                if (enrollExist.FirstOrDefault() != null)
                {
                    for (int i = 0; i < mapperList.Count(); i++)
                    {
                        var chapterResponse = mapperList[i];

                        var lessonList = await _unitOfWork.LessonRepository.Get(lesson => lesson.ChapterId == chapterResponse.Id);

                        var progressList = await _unitOfWork.EnrollmentProgressRepository.Get(progress =>
                                            progress.Lesson.Chapter.Id == chapterResponse.Id &&
                                            progress.Enrollment.BaseUserId == _claimInterface.GetCurrentUserId);

                        chapterResponse.IsDone = !lessonList.Any() ? true : (lessonList.Count() == progressList.Count());
                    }

                    try
                    {
                        var numberOfEnrollment = await _unitOfWork.EnrollmentRepository.CountAsync(filter: enroll => enroll.SubjectCurriculumId == enrollExist.First().SubjectCurriculumId);

                        subject.NumberEnrollment = numberOfEnrollment;
                        subject.IsEnroll = true;

                        var progresses = await _unitOfWork.EnrollmentProgressRepository.Get(filter: progress => progress.EnrollmentId == enrollExist.FirstOrDefault()!.Id
                                                                                                            && progress.Lesson.DeletedAt == null
                                                                                                            && progress.Lesson.Chapter.DeletedAt == null
                                                                                                            && progress.Lesson.Chapter.SubjectCurriculum.Subject!.DeletedAt == null,
                                                                                           includeProperties: [progress => progress.Lesson,
                                                                                                                progress => progress.Lesson.Chapter,
                                                                                                                progress => progress.Lesson.Chapter.SubjectCurriculum,
                                                                                                                progress => progress.Lesson.Chapter.SubjectCurriculum.Subject!]);

                        subject.EnrollmentProgress = new EnrollmentProgressModel();

                        if (progresses.Any())
                        {
                            progresses = progresses.OrderByDescending(progress => progress.UpdatedAt);

                            subject.EnrollmentProgress.LastViewedLesson = _mapper.Map<EntitySimpleModel>(progresses.FirstOrDefault()!.Lesson);

                            subject.EnrollmentProgress.LastViewedChapter = _mapper.Map<EntitySimpleModel>(progresses.FirstOrDefault()!.Lesson.Chapter);

                            var allSubjectLessonCount = await _unitOfWork.LessonRepository.CountAsync(filter: lesson => lesson.Chapter.SubjectCurriculum.SubjectId == subject.Id && lesson.Chapter.SubjectCurriculum.CurriculumId == request.CurriculumId
                                                                                                                    && lesson.DeletedAt == null
                                                                                                                    && lesson.Chapter.DeletedAt == null
                                                                                                                    && lesson.Chapter.SubjectCurriculum.Subject!.DeletedAt == null);

                            subject.EnrollmentProgress.SubjectProgressPercent = (float)Math.Round((double)progresses.Count() / allSubjectLessonCount, 2) * 100;
                        }
                        else
                        {
                            subject.EnrollmentProgress.SubjectProgressPercent = 0;
                            subject.EnrollmentProgress.LastViewedChapter = null;
                            subject.EnrollmentProgress.LastViewedLesson = null;
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    subject.EnrollmentProgress = null;
                }
            }
        }
        return new PagedChapterResponseModelWithProgress
        {
            SubjectModel = subject,
            Items = new PagedList<ChapterResponseModel>(mapperList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize),
            
        };
    }
}