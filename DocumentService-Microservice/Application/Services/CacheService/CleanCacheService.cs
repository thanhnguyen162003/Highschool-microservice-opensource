using Application.Services.CacheService.Intefaces;

namespace Application.Services.CacheService;

public class CleanCacheService(IOrdinaryDistributedCache cache) : ICleanCacheService
{
    /// <summary>
    /// Clear Subject Cache
    /// </summary>
    public async Task ClearRelatedCache()
    {
        var cachePatterns = new[] { "subjects:*" , "subject:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in cache.ScanAsync(pattern))
            {
                await cache.RemoveAsync(key);
            }
        }
    }

    /// <summary>
    /// Clear School Cache
    /// </summary>
    public async Task ClearRelatedCacheSchool()
    {
        var cachePatterns = new[] { "schools:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in cache.ScanAsync(pattern))
            {
                await cache.RemoveAsync(key);
            }
        }
    }

    /// <summary>
    /// Clear Provinces Cache
    /// </summary>
    public async Task ClearRelatedCacheProvince()
    {
        var cachePatterns = new[] { "provinces:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in cache.ScanAsync(pattern))
            {
                await cache.RemoveAsync(key);
            }
        }
    }

    /// <summary>
    /// Clear Subject With User Cache
    /// </summary>
    public async Task ClearRelatedCacheSpecificUser(Guid? userId)
    {
        var cachePatterns = new[] { $"subject:{userId}:*" };
        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in cache.ScanAsync(pattern))
            {
                await cache.RemoveAsync(key);
            }
        }
    }
    /// <summary>
    /// Clear Subject with SubjectId Cache
    /// </summary>
    public async Task ClearRelatedCacheSpecificId(Guid subjectId)
    {
        var cachePatterns = new[] { $"subject:{subjectId}:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in cache.ScanAsync(pattern))
            {
                await cache.RemoveAsync(key);
            }
        }
    }
    /// <summary>
    /// Clear Subject with Slug Cache
    /// </summary>
    public async Task ClearRelatedCacheSpecificSlug(string slug)
    {
        var cachePatterns = new[] { $"subject:{slug}:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in cache.ScanAsync(pattern))
            {
                await cache.RemoveAsync(key);
            }
        }
    }
}