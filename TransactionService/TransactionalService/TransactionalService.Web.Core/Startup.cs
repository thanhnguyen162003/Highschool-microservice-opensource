using System;
using Furion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Carter;
using TransactionalService.Application.Common.Interfaces.PayOsInterface;
using TransactionalService.Application.Services.PayOsService;
using TransactionalService.Core.Configurations;

namespace TransactionalService.Web.Core
{
	public class Startup : AppStartup
	{
		private readonly IConfiguration _configuration;
		public Startup()
		{
		}

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddConsoleFormatter();
			services.AddCarter();
			services.AddJwt<JwtHandler>();
			services.AddScoped<IPayOsService, PayOsService>();
			services.Configure<PayOsSettings>(_configuration.GetSection("PayOS"));
			

			services.AddCorsAccessor();

			services.AddControllers()
					.AddInjectWithUnifyResult();

			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					builder =>
					{
						builder.AllowAnyOrigin()
							.AllowAnyMethod()
							.AllowAnyHeader()
							.WithExposedHeaders("Location", "X-Pagination")
							.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
					});
			});

			// services.AddAuthentication(options =>
			// {
			// 	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			// 	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			// })
			// .AddJwtBearer(options =>
			// {
			// 	options.TokenValidationParameters = new TokenValidationParameters
			// 	{
			// 		ValidateIssuer = true,
			// 		ValidateAudience = true,
			// 		ValidateLifetime = true,
			// 		ValidateIssuerSigningKey = true,
			// 		ValidIssuer = _configuration["JWTSetting:ValidIssuer"],
			// 		ValidAudience = _configuration["JWTSetting:ValidAudience"],
			// 		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSetting:SecurityKey"]))
			// 	};
			//
			// });
			//
			services.AddAuthorization(options =>
			{
				options.AddPolicy("studentPolicy", policy =>
					policy.RequireAuthenticatedUser().RequireClaim("Role", "4"));
				options.AddPolicy("teacherPolicy", policy =>
					policy.RequireAuthenticatedUser().RequireClaim("Role", "3"));
				options.AddPolicy("moderatorPolicy", policy =>
					policy.RequireAuthenticatedUser().RequireClaim("Role", "2"));
				options.AddPolicy("adminPolicy", policy =>
					policy.RequireAuthenticatedUser().RequireClaim("Role", "1"));
			});

		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors("AllowAll");

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCorsAccessor();

			app.UseAuthentication();
			app.UseAuthorization();
			
			app.UseInject(string.Empty);

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapCarter();
			});
		}
	}
}
