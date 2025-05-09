namespace Application.Services.CacheService.Intefaces;

public interface ICacheOption
{
    Task InvalidateCacheSubjectIdAsync(Guid? userId,Guid subjectId, CancellationToken cancellationToken = default);
    Task InvalidateCacheSubjectSlugAsync(Guid? userId,string subjectSlug, CancellationToken cancellationToken = default);
}