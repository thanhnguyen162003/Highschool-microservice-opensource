using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.LessonModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.LessonFeature.Queries;

public record LessonDetailQuery : IRequest<LessonDetailResponseModel>
{
    public Guid LessonId;
}

public class LessonDetailQueryHandler(
    IUnitOfWork unitOfWork, 
    IMapper mapper)
    : IRequestHandler<LessonDetailQuery, LessonDetailResponseModel>
{
    public async Task<LessonDetailResponseModel> Handle(LessonDetailQuery request, CancellationToken cancellationToken)
    {
        var lesson = await unitOfWork.LessonRepository.GetById(request.LessonId);
        if (lesson == null)
        {
            throw new KeyNotFoundException($"Lesson with ID {request.LessonId} not found.");
        }
        
        var nextLessonList = await unitOfWork.LessonRepository.Get(x => x.ChapterId == lesson.ChapterId);

        var mapData = mapper.Map<LessonDetailResponseModel>(lesson);
        mapData.VideoUrl = mapData.YoutubeVideoUrl;
        mapData.NextLessonId = nextLessonList.Where(x => x.DisplayOrder > mapData.DisplayOrder).OrderBy(x => x.DisplayOrder).FirstOrDefault()?.Id ?? null;
        if (mapData.NextLessonId == null)
        {
            int.TryParse(lesson.Chapter.ChapterLevel, out var currentChapterLevel);
            var chapterList = await unitOfWork.ChapterRepository.Get(x => x.SubjectCurriculumId == lesson.Chapter.SubjectCurriculumId);
            var sortedChapters = chapterList.Where(x => int.TryParse(x.ChapterLevel, out var level) && level > currentChapterLevel)
                                            .OrderBy(c =>
                                            {
                                                return int.TryParse(c.ChapterLevel, out int level) ? level : int.MaxValue;
                                            })
                                            .ToList();

            mapData.NextChapterId = sortedChapters.FirstOrDefault()?.Id ?? null;
        }
        return mapData;
    }
}
