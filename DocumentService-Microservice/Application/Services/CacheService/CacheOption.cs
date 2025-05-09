using Application.Services.CacheService.Intefaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services.CacheService;

public class CacheOption(IOrdinaryDistributedCache cache) : ICacheOption
{
    public async Task InvalidateCacheSubjectIdAsync(Guid? userId, Guid subjectId, CancellationToken cancellationToken = default)
    {
        string subjectKey = $"subject-{subjectId}-{userId}";
        string? cachedSubjectKey = await cache.GetStringAsync(subjectKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedSubjectKey))
        {
            await cache.RemoveAsync(subjectKey, cancellationToken);
        }
    }

    public async Task InvalidateCacheSubjectSlugAsync(Guid? userId, string subjectSlug, CancellationToken cancellationToken = default)
    {
        string subjectSlugKey = $"subject-{subjectSlug}-{userId}";
        string? cachedSubjectSlugKey = await cache.GetStringAsync(subjectSlugKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedSubjectSlugKey))
        {
            await cache.RemoveAsync(subjectSlugKey, cancellationToken);
        }
    }
}