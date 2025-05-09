namespace Application.Services.CacheService.Intefaces;

public interface ICleanCacheService
{
    Task ClearRelatedCache();
    Task ClearRelatedCacheSchool();
    Task ClearRelatedCacheProvince();
    Task ClearRelatedCacheSpecificUser(Guid? userId);
    Task ClearRelatedCacheSpecificId(Guid subjectId);
    Task ClearRelatedCacheSpecificSlug(string slug);
}
