using Application;
using Application.Common.Exceptions;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Configurations;
using Application.Common.Interfaces;
using Application.Services;
using Novu.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddWebServices();
builder.Services.AddCarter();
builder.Services.AddSignalR();
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 3 * 1024 * 1024;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 3 * 1024 * 1024;
});

builder.Services.RegisterNovuClients(builder.Configuration).AddTransient<INovuService, NovuService>();

builder.Services.AddRedis(builder.Configuration);

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

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWTSetting:ValidIssuer"],
            ValidAudience = builder.Configuration["JWTSetting:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSetting:SecurityKey"]))
        };

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

var app = builder.Build();
//app.MapHub<BaseNotificationHub>("/hubs/base");
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseExceptionHandler(options => { });
app.UseHttpsRedirection();
app.MapCarter();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
