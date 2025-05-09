using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.LessonModel;
using Application.Common.UoW;
using Application.Constants;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.LessonFeature.Queries;

public record LessonQuery : IRequest<PagedList<LessonModel>>
{
    public required LessonQueryFilter QueryFilter;
    public Guid ChapterId;
}

public class LessonQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions, IClaimInterface claimInterface)
    : IRequestHandler<LessonQuery, PagedList<LessonModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<LessonModel>> Handle(LessonQuery request, CancellationToken cancellationToken)
    {
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;
        
        var (lessons, totalCount) = await unitOfWork.LessonRepository.GetLessonsByFilters(request.ChapterId, request.QueryFilter);
        if (!lessons.Any())
        {
            return new PagedList<LessonModel>(new List<LessonModel>(), 0, 0, 0);
        }

        var mappedList = mapper.Map<List<LessonModel>>(lessons);
        mappedList = mappedList.Select(lesson => { lesson.VideoUrl = lesson.YoutubeVideoUrl; return lesson; }).ToList();

        if (claimInterface.GetCurrentUserId != Guid.Empty && claimInterface.GetRole.Contains(RoleConstaint.STUDENT.ToString(), StringComparison.Ordinal))
        {
            var subjectCurriculumId = lessons.FirstOrDefault()!.Chapter.SubjectCurriculum.Id;

            var checkEnroll = await unitOfWork.EnrollmentRepository.Get(enroll => enroll.SubjectCurriculumId == subjectCurriculumId && enroll.BaseUserId == claimInterface.GetCurrentUserId);

            var enrollment = checkEnroll.FirstOrDefault();

            if (enrollment != null)
            {
                var allProgress = await unitOfWork.EnrollmentProgressRepository.Get(progress => progress.EnrollmentId == enrollment.Id);

                for (int i = 0; i < mappedList.Count(); i++)
                {
                    var lessonModel = mappedList.ElementAt(i);

                    var lessonDone = allProgress.Where(progress => progress.LessonId == lessonModel.Id);

                    mappedList.ElementAt(i).IsDone = lessonDone.Any();

                    mappedList.ElementAt(i).IsCurrentView = false;
                }              

                if (allProgress.Any())
                {
                    var currentLessonId = allProgress.MaxBy(o => o.UpdatedAt)?.LessonId;

                    var lessonInMappedList = mappedList.Where(lessonModel => lessonModel.Id == currentLessonId).FirstOrDefault();

                    if (lessonInMappedList != null)
                    {
                        lessonInMappedList.IsCurrentView = true;
                    }              
                }
            }
        }
        return new PagedList<LessonModel>(mappedList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        
    }
}
