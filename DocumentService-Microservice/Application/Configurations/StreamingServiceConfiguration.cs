using static Application.Endpoints.AIEndpoints;
using System.Reactive.Subjects;

namespace Application.Configurations
{
	public static class StreamingServiceConfiguration
	{
		public static IServiceCollection AddStreamingServices(this IServiceCollection services)
		{
			services.AddSingleton<Subject<object>>();
			return services;
		}
	}
}
