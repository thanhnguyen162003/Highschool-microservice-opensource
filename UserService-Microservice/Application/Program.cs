using Application;
using Application.Common.Models.Settings;
using Carter;
using Domain;
using Domain.Configurations;
using Domain.Settings;
using FAI.API.Middleware;
using Infrastructure.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddWebServices(builder.Configuration);
builder.Services.AddCarter();
builder.WebHost.ConfigureKestrel(options =>
{
	options.ConfigureHttpsDefaults(httpsOptions =>
	{
		httpsOptions.AllowAnyClientCertificate();
	});
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();


// Get data from appsettings.json
var defaultSystem = builder.Configuration.GetSection("DefaultSystem").Get<DefaultSystem>();
builder.Services.Configure<DefaultSystem>(builder.Configuration.GetSection("DefaultSystem"));
builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("CloudinarySetting"));
builder.Services.Configure<AISetting>(builder.Configuration.GetSection("AISetting"));

//Set size limit for request
builder.Services.Configure<KestrelServerOptions>(options =>
{
	options.Limits.MaxRequestBodySize = defaultSystem?.LimitSizeFile; // 1GB
});

// Set up caching Redis
builder.Services.AddRedis(builder.Configuration);

// Set up maill
builder.Services.AddFluentEmail(builder.Configuration);

// Set up JWT
builder.Services.AddJWT(builder.Configuration);

// Set up CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		builder =>
		{
			builder.AllowAnyOrigin()
				   .AllowAnyMethod()
				   .AllowAnyHeader()
				   .WithExposedHeaders("Location", "X-Pagination");
		});
});

builder.Services.AddAuthorization(options =>
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

// Set up auto mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Set up database
builder.Services.AddDbContext<UserDatabaseContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.ConfigureExceptionHandler();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapCarter();

app.UseAuthentication();

app.UseAuthorization();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
