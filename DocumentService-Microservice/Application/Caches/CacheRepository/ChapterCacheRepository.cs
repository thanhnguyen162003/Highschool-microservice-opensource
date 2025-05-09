using Application.Services.CacheService.Intefaces;
using Domain.CustomModel;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;

namespace Application.Caches.CacheRepository;

public class ChapterCacheRepository(
    ChapterRepository decorated,
    IOrdinaryDistributedCache primaryCache,
    DocumentDbContext context)
    : BaseRepository<Chapter>(context), IChapterRepository

{
    private readonly ChapterRepository _decorated = decorated as ChapterRepository ?? throw new ArgumentException("Decorated repository must be of type ChapterRepository", nameof(decorated));
    private readonly DocumentDbContext _context = context;

    public Task<bool> ChapterIdExistsAsync(Guid? guid)
    {
        return _decorated.ChapterIdExistsAsync(guid);
    }

    public Task<bool> ChapterNameExistsAsync(string name)
    {
        return _decorated.ChapterNameExistsAsync(name);
    }

    public async Task<bool> CreateChapter(Chapter dto)
    {
        // var result = await _decorated.CreateChapter(dto);
        // if (result)
        // {
        //     await primaryCache.RemoveAsync($"all-chapter-{dto.SubjectId}");
        //     await primaryCache.RemoveAsync($"chapter-subject-{dto.SubjectId}");
        // }
        // return result;
        return await _decorated.CreateChapter(dto);
    }

    public async Task<bool> CreateChapterList(List<Chapter> listChapters)
    {
        // var result = await _decorated.CreateChapterList(listChapters);
        // if (result)
        // {
        //     await primaryCache.RemoveAsync($"all-chapter-{listChapters[0].SubjectId}");
        //     await primaryCache.RemoveAsync($"chapter-subject-{listChapters[0].SubjectId}");
        // }
        // return result;
        return await _decorated.CreateChapterList(listChapters);
    }

    public async Task<bool> DeleteChapter(Guid id)
    {
        // var result = await _decorated.DeleteChapter(id);
        // if (result)
        // {
        //     await primaryCache.RemoveAsync($"all-chapter-{GetChapterByChapterId(id).Result.SubjectId}");
        //     await primaryCache.RemoveAsync($"chapter-subject-{GetChapterByChapterId(id).Result.SubjectId}");
        // }
        // return result;
        return await _decorated.DeleteChapter(id);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculumIdModerator(ChapterQueryFilter queryFilter, Guid subjectCurriculumId)
    {
        return await _decorated.GetChaptersBySubjectCurriculumIdModerator(queryFilter, subjectCurriculumId);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculum(ChapterQueryFilter queryFilter, Guid curriculumId, Guid subjectId)
    {
       return await _decorated.GetChaptersBySubjectCurriculum(queryFilter, curriculumId, subjectId);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculumModerator(ChapterQueryFilter queryFilter, Guid curriculumId, Guid subjectId)
    {
        return await _decorated.GetChaptersBySubjectCurriculumModerator(queryFilter, curriculumId, subjectId);
    }

    public async Task<ChapterModel> GetChapterByChapterId(Guid id)
    {
        return await _decorated.GetChapterByChapterId(id);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChapters(ChapterQueryFilter queryFilter)
    {
        // //remove later- error cache
        // string key = $"all-chapter-{queryFilter.PageSize}-{queryFilter.PageNumber}-{queryFilter.Search}-{userId}";
        // string? cachedChapters = await primaryCache.GetStringAsync(key);
        //
        // if (!string.IsNullOrEmpty(cachedChapters))
        // {
        //     return JsonConvert.DeserializeObject<List<Chapter>>(cachedChapters)!; // Because of redis store data in object. so we need to deserialize to get data
        // }
        // var cacheOptions = new DistributedCacheEntryOptions
        // {
        //     AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        // }; 
        // var loopHandling = new JsonSerializerSettings 
        // { 
        //     ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        // };
        // var chapters = await _decorated.GetChapters(queryFilter);
        // await primaryCache.SetStringAsync(key, JsonConvert.SerializeObject(chapters,loopHandling),cacheOptions);
        // return chapters;
        return await _decorated.GetChapters(queryFilter);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersModerator(ChapterQueryFilter queryFilter)
    {
        return await _decorated.GetChaptersModerator(queryFilter); 
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubject(ChapterQueryFilter queryFilter, Guid subjectId)
    {
        // string key = $"chapter-subject-{subjectId}-{queryFilter.PageSize}-{queryFilter.PageNumber}-{queryFilter.Search}";
        // string? cachedChapters = await primaryCache.GetStringAsync(key);
        // if (!string.IsNullOrEmpty(cachedChapters))
        // {
        //     return JsonConvert.DeserializeObject<List<Chapter>>(cachedChapters)!; // Because of redis store data in object. so we need to deserialize to get data
        // }
        // var cacheOptions = new DistributedCacheEntryOptions
        // {
        //     AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
        // }; 
        // var loopHandling = new JsonSerializerSettings 
        // { 
        //     ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        // };
        // var chapters = await _decorated.GetChaptersBySubject(queryFilter, subjectId);
        // await primaryCache.SetStringAsync(key, JsonConvert.SerializeObject(chapters,loopHandling),cacheOptions);
        // return chapters;
        return await _decorated.GetChaptersBySubject(queryFilter, subjectId);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculumId(ChapterQueryFilter queryFilter, Guid subjectCurriculumId)
    {
        return await _decorated.GetChaptersBySubjectCurriculumId(queryFilter, subjectCurriculumId);
    }

    public async Task<bool> UpdateChapter(Chapter chapter)
    {
        // var result = await _decorated.UpdateChapter(chapter);
        // if (result)
        // {
        //     await primaryCache.RemoveAsync($"all-chapter-{chapter.SubjectId}");
        //     await primaryCache.RemoveAsync($"chapter-subject-{chapter.SubjectId}");
        // }
        // return result;
        return await _decorated.UpdateChapter(chapter);
    }

    public IEnumerable<CourseQueryModel> GetChapters()
    {
        return _decorated.GetChapters();
    }
}