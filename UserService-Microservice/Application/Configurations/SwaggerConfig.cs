using Microsoft.OpenApi.Models;


namespace Domain.Configuration
{
	public static class SwaggerConfig
	{
		public static void AddSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(swagger =>
			{
				swagger.SwaggerDoc("v1", new() { Title = "Event Management", Version = "v1" });
				swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter a valid token using the Bearer scheme (\"bearer {token}\")",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "Security",
					Scheme = "Bearer"
				});
				swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
				}
			},
			new string[]{}
		}
	});

			});
		}
	}
}
