using Domain.Constants;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations
{
    public static class PolicyConfig
	{
		public static void AddPolicies(this IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.AddPolicy(PolicyType.Moderator, policy => policy.RequireClaim(UserClaimType.Role, ((int)UserEnum.Moderator).ToString()));
				options.AddPolicy(PolicyType.Teacher, policy => policy.RequireClaim(UserClaimType.Role, ((int)UserEnum.Teacher).ToString()));
				options.AddPolicy(PolicyType.Student, policy => policy.RequireClaim(UserClaimType.Role, ((int)UserEnum.Student).ToString()));
				options.AddPolicy(PolicyType.AcademicUser, policy => policy.RequireClaim(UserClaimType.Role, ((int)UserEnum.Student).ToString(), ((int)UserEnum.Teacher).ToString()));
			});
		}
	}
}
