namespace Application.Common.Interfaces.CacheInterface
{
	public interface ICleanCacheService
	{
		Task ClearRelatedCacheUniversity();
		Task ClearRelatedCacheUniversitySave();
	}
}
