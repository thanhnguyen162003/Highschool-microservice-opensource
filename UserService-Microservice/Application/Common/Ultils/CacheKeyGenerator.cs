using Infrastructure.QueryFilters;
using Newtonsoft.Json;

namespace Application.Common.Ultils
{
	public static class CacheKeyGenerator
	{
		public static string GenerateUniversityListKey(UniversityQueryFilter filter)
		{
			return $"universities:{JsonConvert.SerializeObject(filter)}";
		}
	}
}
