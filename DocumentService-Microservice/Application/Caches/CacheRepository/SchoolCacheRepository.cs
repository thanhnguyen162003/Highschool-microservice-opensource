using Application.Services.CacheService.Intefaces;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Application.Caches.CacheRepository;

public class SchoolCacheRepository(SchoolRepository decorated,
IOrdinaryDistributedCache primaryCache,
ICleanCacheService cleanCacheService,
DocumentDbContext context)
: BaseRepository<School>(context), ISchoolRepository
{
    public async Task<bool> CreateSchoolAsync(School school)
    {
        return await decorated.CreateSchoolAsync(school);;
    }

    public async Task<bool> DeleteSchool(Guid id)
    {
        await cleanCacheService.ClearRelatedCacheSchool();
        return await decorated.DeleteSchool(id);;
    }

    public async Task<(List<School> Schools, int TotalCount)> GetSchoolsAsync(SchoolQueryFilter queryFilter)
    {
        string key = $"schools:{JsonConvert.SerializeObject(queryFilter)}";
        string? cacheSchools = await primaryCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cacheSchools))
        {
            var cachedResult = JsonConvert.DeserializeObject<(List<School> Schools, int TotalCount)>(cacheSchools,
                new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            
            return cachedResult;
        }
        var result = await decorated.GetSchoolsAsync(queryFilter);
        if (result.Schools != null && result.Schools.Any())
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
            var jsonSettings = new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string serializedResult = JsonConvert.SerializeObject(result, jsonSettings);
            await primaryCache.SetStringAsync(key, serializedResult, cacheOptions);
        }
        return result;
    }

    public async Task<(List<School> Schools, int TotalCount)> GetSchoolsByProvinceIdAsync(int id, SchoolQueryFilter queryFilter)
    {
        string key = $"schools:{id}:{JsonConvert.SerializeObject(queryFilter)}";
        string? cacheSchools = await primaryCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cacheSchools))
        {
            var cachedResult = JsonConvert.DeserializeObject<(List<School> Schools, int TotalCount)>(cacheSchools,
                new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            
            return cachedResult;
        }
        var result = await decorated.GetSchoolsByProvinceIdAsync(id, queryFilter);
        if (result.Schools != null && result.Schools.Any())
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
            await primaryCache.SetStringAsync(key, serializedResult, cacheOptions);
        }
        return result;
    }
}