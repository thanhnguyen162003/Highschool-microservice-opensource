using System.Text;
using Algolia.Search.Models.Search;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Application;
using Application.Common.Exceptions;
using Application.Configurations;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWebServices();
builder.Services.AddControllers();
builder.Services.AddCarter();

//builder.Services.AddGrpc();
builder.Services.AddDaprClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Location", "X-Pagination", "X-Flashcards-Pagination", "X-Subjects-Pagination",
                "X-Documents-Pagination", "X-Tips-Pagination", "X-Folders-Pagination", "X-Names-Pagination");
        });
});
// Register Redis cache
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
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

builder.Services.AddAWSService<IAmazonS3>();
builder.Services.Configure<AWSOptions>(builder.Configuration.GetSection("AWS"));

var app = builder.Build();
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseScalar();
}
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseExceptionHandler(options => { });
app.UseHttpsRedirection();
app.MapCarter();
app.UseAuthentication();
app.MapControllers();
app.UseAuthorization();
app.Run();
