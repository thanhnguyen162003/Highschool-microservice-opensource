using System.Threading;
using Application.Common.Interfaces.ClaimInterface;
using Application.Services.CacheService.Intefaces;
using Domain.CustomModel;
using Domain.Entities;
using Domain.Enums;
using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Application.Caches.CacheRepository;

public class SubjectCacheRepository(
    SubjectRepository decorated,
    IOrdinaryDistributedCache primaryCache,
    ICleanCacheService cleanCacheService,
    IClaimInterface claimInterface,
    DocumentDbContext context)
    : BaseRepository<Subject>(context), ISubjectRepository

{
    private readonly SubjectRepository _decorated = decorated as SubjectRepository ?? throw new ArgumentException("Decorated repository must be of type SubjectRepository", nameof(decorated));
    private readonly DocumentDbContext _context = context;
    
    public async Task<(List<Subject> Subjects, int TotalCount)> GetSubjects(SubjectQueryFilter queryFilter, CancellationToken cancellationToken = default)
    {
        var userId = claimInterface.GetCurrentUserId;
        string key = $"subjects:{userId}:{JsonConvert.SerializeObject(queryFilter)}";
        string? cacheSubjects = await primaryCache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(cacheSubjects))
        {
            var cachedResult = JsonConvert.DeserializeObject<(List<Subject> Subjects, int TotalCount)>(cacheSubjects,
                new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            
            return cachedResult;
        }
        var result = await _decorated.GetSubjects(queryFilter, cancellationToken);
        if (result.Subjects != null && result.Subjects.Any())
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
            };
            var jsonSettings = new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string serializedResult = JsonConvert.SerializeObject(result, jsonSettings);
            await primaryCache.SetStringAsync(key, serializedResult, cacheOptions, cancellationToken);
        }
        return result;
    }


    public async Task<SubjectModel> GetSubjectBySubjectId(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = claimInterface.GetCurrentUserId;
        string key = $"subject:{userId}:{id}";
        string? cacheSubject = await primaryCache.GetStringAsync(key,cancellationToken);
        SubjectModel subject;
        if (string.IsNullOrEmpty(cacheSubject))
        {
            subject = await _decorated.GetSubjectBySubjectId(id, cancellationToken);
            if (subject is null)
            {
                return subject;
            }
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4)
            };
            await primaryCache.SetStringAsync(key, JsonConvert.SerializeObject(subject),cacheOptions, cancellationToken);
            return subject;
        }
        subject = JsonConvert.DeserializeObject<SubjectModel>(cacheSubject,
            // tell that it need to find constructor that dont have public or private default
            new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
        )!;
        return subject;
    }
    
    public async Task<SubjectModel> GetSubjectUnPublishBySubjectSlug(string slug, CancellationToken cancellationToken = default)
    {
        return await _decorated.GetSubjectUnPublishBySubjectSlug(slug, cancellationToken);
    }

    public async Task<SubjectModel> GetSubjectBySubjectSlugAndCurriculum(string slug, Guid curriculumId,
        CancellationToken cancellationToken = default)
    {
        var userId = claimInterface.GetCurrentUserId;
        string key = $"subject:{userId}:{slug}:{curriculumId}";
        string? cacheSubject = await primaryCache.GetStringAsync(key, cancellationToken);
        SubjectModel subject;
        if (string.IsNullOrEmpty(cacheSubject))
        {
            subject = await _decorated.GetSubjectBySubjectSlugAndCurriculum(slug, curriculumId, cancellationToken);
            if (subject is null)
            {
                return subject;
            }
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4)
            };
            await primaryCache.SetStringAsync(key, JsonConvert.SerializeObject(subject),cacheOptions, cancellationToken);
            return subject;
        }
        subject = JsonConvert.DeserializeObject<SubjectModel>(cacheSubject,
            new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
        )!;
        return subject;
    }

    public async Task<bool> DeleteSubject(Guid id, CancellationToken cancellationToken)
    {
        await cleanCacheService.ClearRelatedCache();
        return await _decorated.DeleteSubject(id, cancellationToken);;
    }

    public async Task<SubjectModel> GetSubjectUnPublishBySubjectId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _decorated.GetSubjectUnPublishBySubjectId(id, cancellationToken);
    }

    public async Task<bool> UpdateSubject(Subject dto, CancellationToken cancellationToken)
    {
        await cleanCacheService.ClearRelatedCache();
        return await _decorated.UpdateSubject(dto, cancellationToken);;
    }

    public async Task<bool> CreateSubject(Subject dto, CancellationToken cancellationToken)
    {
		await cleanCacheService.ClearRelatedCache();
		var response = await _decorated.CreateSubject(dto, cancellationToken);
        return response;
    }

    public async Task<SubjectModel> GetSubjectBySubjectSlug(string slug, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        string key = $"subject:{userId}:{slug}";
        string? cacheSubject = await primaryCache.GetStringAsync(key, cancellationToken);
        SubjectModel subject;
        if (string.IsNullOrEmpty(cacheSubject))
        {
            subject = await _decorated.GetSubjectBySubjectSlug(slug, cancellationToken);
            if (subject is null)
            {
                return subject;
            }
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4)
            };
            await primaryCache.SetStringAsync(key, JsonConvert.SerializeObject(subject),cacheOptions, cancellationToken);
            return subject;
        }
        subject = JsonConvert.DeserializeObject<SubjectModel>(cacheSubject,
            new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
        )!;
        return subject;
    }

    public async Task<bool> SubjectIdExistAsync(Guid? guid)
    {
        var response = await _decorated.SubjectIdExistAsync(guid);
        return response;
    }

    public async Task<bool> SubjectNameExistAsync(string name)
    {
        var response = await _decorated.SubjectNameExistAsync(name);
        return response;
    }

	public async Task<IEnumerable<string>> CheckSubjectName(IEnumerable<string> subjectNames)
	{
        var response = await _decorated.CheckSubjectName(subjectNames);
        return response;
	}

    public async Task<List<Subject>> GetPlaceHolderSubjects()
    {
        // string key = $"subject:placeholder";
        // string? cacheSubject = await primaryCache.GetStringAsync(key);
        // List<Subject> subject;
        // if (string.IsNullOrEmpty(cacheSubject))
        // {
        //     subject = await _decorated.GetPlaceHolderSubjects();
        //     if (subject is null)
        //     {
        //         return subject;
        //     }
        //     var cacheOptions = new DistributedCacheEntryOptions
        //     {
        //         AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        //     };
        //     await primaryCache.SetStringAsync(key, JsonConvert.SerializeObject(subject),cacheOptions);
        //     return subject;
        // }
        // subject = JsonConvert.DeserializeObject<List<Subject>>(cacheSubject,
        //     new JsonSerializerSettings
        //     {
        //         ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        //         ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //     }
        // )!;
        // return subject;
        return await _decorated.GetPlaceHolderSubjects();
    }

    public async Task UpdateViewCount(Guid id)
    {
        await _decorated.UpdateViewCount(id);
    }

    public async Task<List<Subject>> GetSubjectsRelatedClass(string? className,
        CancellationToken cancellationToken = default)
    {
        return await _decorated.GetSubjectsRelatedClass(className, cancellationToken);
    }

    public async Task<List<Subject>> GetAdditionalSubjects(string className, int count, CancellationToken cancellationToken)
    {
        return await _decorated.GetAdditionalSubjects(className, count, cancellationToken);
    }

	public async Task<List<Subject>> GetSubjects(CancellationToken cancellationToken = default)
	{
        return await _decorated.GetSubjects(cancellationToken);
	}

    public async Task<List<string>> GetSubjectIdsAsString(CancellationToken cancellationToken = default)
    {
        return await _decorated.GetSubjectIdsAsString(cancellationToken);
    }
    
    public async Task<Dictionary<string, string>> GetGrade(List<string> subjectId, CancellationToken cancellationToken = default)
    {
        return await _decorated.GetGrade(subjectId, cancellationToken);
    }

    public IEnumerable<CourseQueryModel> GetSubjectsSearch()
    {
        return _decorated.GetSubjectsSearch();
    }

	public async Task<List<Subject>> GetAllSubjects(CancellationToken cancellationToken = default)
	{
		return await _decorated.GetAllSubjects(cancellationToken);
	}

    public async Task<List<Subject>> GetSubjectsByMasterSubjectId(Guid masterSubjectId, int? grade, CancellationToken cancellationToken)
    {
        return await _decorated.GetSubjectsByMasterSubjectId(masterSubjectId, grade, cancellationToken);
    }

    public async Task<List<Subject>> GetAdditionalSubjectsByCategory(int count, int? grade, CancellationToken cancellationToken)
    {
        return await _decorated.GetAdditionalSubjectsByCategory(count, grade, cancellationToken);
    }

    public async Task<List<Subject>> GetPlaceHolderSubjects(int? grade = null)
    {
        return await _decorated.GetPlaceHolderSubjects(grade);
    }
}