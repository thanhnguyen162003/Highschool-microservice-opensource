using Application;
using Application.Middlewares;
using Carter;
using Domain.Constants;
using Infrastructure;
using Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplication();
builder.AddInfrastructure();

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
