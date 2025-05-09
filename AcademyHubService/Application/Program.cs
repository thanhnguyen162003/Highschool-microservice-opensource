using Application;
using Application.Middlewares;
using Application.Services.CalendarService;
using Domain.Constants;
using Infrastructure;
using Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplication();
builder.AddInfrastructure();
builder.Services.AddDaprClient();
builder.Services.Configure<CalendarSetting>(builder.Configuration.GetSection("CalendarSetting"));

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseScalar();
}

app.UseMiddleware<GlobalException>();

app.UseCors(builder =>
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders(SystemConstant.HeaderPagination));

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
