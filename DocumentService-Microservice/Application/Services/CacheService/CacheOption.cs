using Application.Services.CacheService.Intefaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services.CacheService;

public class CacheOption : ICacheOption
{
    private readonly IOrdinaryDistributedCache _cache;

    public CacheOption(IOrdinaryDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task InvalidateCacheSubjectIdAsync(Guid? userId, Guid subjectId, CancellationToken cancellationToken = default)
    {
        string subjectKey = $"subject-{subjectId}-{userId}";
        string? cachedSubjectKey = await _cache.GetStringAsync(subjectKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedSubjectKey))
        {
            await _cache.RemoveAsync(subjectKey, cancellationToken);
        }
    }

    public async Task InvalidateCacheSubjectSlugAsync(Guid? userId, string subjectSlug, CancellationToken cancellationToken = default)
    {
        string subjectSlugKey = $"subject-{subjectSlug}-{userId}";
        string? cachedSubjectSlugKey = await _cache.GetStringAsync(subjectSlugKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedSubjectSlugKey))
        {
            await _cache.RemoveAsync(subjectSlugKey, cancellationToken);
        }
    }
}