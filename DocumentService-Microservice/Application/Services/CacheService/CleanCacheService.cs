using Application.Services.CacheService.Intefaces;

namespace Application.Services.CacheService;

public class CleanCacheService : ICleanCacheService
{
    private readonly IOrdinaryDistributedCache _cache;

    public CleanCacheService(IOrdinaryDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task ClearRelatedCache()
    {
        var cachePatterns = new[] { "subject:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in _cache.ScanAsync(pattern))
            {
                await _cache.RemoveAsync(key);
            }
        }
    }

    public async Task ClearRelatedCacheSchool()
    {
        var cachePatterns = new[] { "schools:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in _cache.ScanAsync(pattern))
            {
                await _cache.RemoveAsync(key);
            }
        }
    }

    public async Task ClearRelatedCacheProvince()
    {
        var cachePatterns = new[] { "provinces:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in _cache.ScanAsync(pattern))
            {
                await _cache.RemoveAsync(key);
            }
        }
    }

    public async Task ClearRelatedCacheSpecificUser(Guid? userId)
    {
        var cachePatterns = new[] { $"subject:{userId}:*" };
        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in _cache.ScanAsync(pattern))
            {
                await _cache.RemoveAsync(key);
            }
        }
    }
    public async Task ClearRelatedCacheSpecificId(Guid subjectId)
    {
        var cachePatterns = new[] { $"subject:{subjectId}:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in _cache.ScanAsync(pattern))
            {
                await _cache.RemoveAsync(key);
            }
        }
    }
    public async Task ClearRelatedCacheSpecificSlug(string slug)
    {
        var cachePatterns = new[] { $"subject:{slug}:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in _cache.ScanAsync(pattern))
            {
                await _cache.RemoveAsync(key);
            }
        }
    }
}