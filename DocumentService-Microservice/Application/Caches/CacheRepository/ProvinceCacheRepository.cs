using Application.Services.CacheService.Intefaces;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Application.Caches.CacheRepository;

public class ProvinceCacheRepository(ProvinceRepository decorated,
    IOrdinaryDistributedCache primaryCache,
    ICleanCacheService cleanCacheService,
    DocumentDbContext context)
    : BaseRepository<Province>(context), IProvinceRepository
{
    public async Task<bool> CreateProvinceAsync(Province province)
    {
        await cleanCacheService.ClearRelatedCacheProvince();
        return await decorated.CreateProvinceAsync(province);
    }

    public async Task<bool> DeleteProvince(Guid id)
    {
        await cleanCacheService.ClearRelatedCacheProvince();
        return await decorated.DeleteProvince(id);
    }

    public async Task<(List<Province> Provinces, int TotalCount)> GetProvinceAsync(ProvinceQueryFilter queryFilter)
    {
        string key = $"provinces:{JsonConvert.SerializeObject(queryFilter)}";
        string? cacheProvinces = await primaryCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cacheProvinces))
        {
            var cachedResult = JsonConvert.DeserializeObject<(List<Province> Provinces, int TotalCount)>(cacheProvinces,
                new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            
            return cachedResult;
        }
        var result = await decorated.GetProvinceAsync(queryFilter);
        if (result.Provinces != null && result.Provinces.Any())
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
}