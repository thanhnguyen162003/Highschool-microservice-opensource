using Domain.Common.Security;
using Domain.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Domain.Configurations
{
	public static class JWTConfig
	{
		public static void AddJWT(this IServiceCollection services, IConfiguration configuration)
		{
            services.Configure<JWTSetting>(configuration.GetSection("JWTSetting"));
            var jwtSettings = configuration.GetSection("JWTSetting").Get<JWTSetting>();

			services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidIssuer = jwtSettings?.Issuer,
					ValidAudience = jwtSettings?.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecurityKey!)),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true
                };
			});

		}
	}
}
