using Application;
using Application.Common.Ultils;
using Carter;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Configurations;
using Application.Services.AIService;
using Application.Services.CalendarService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddWebServices();
builder.Services.AddCarter();

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 3 * 1024 * 1024; 
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 3 * 1024 * 1024;
});

//RegisterCloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

// Register Redis cache
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

builder.Services.Configure<AISetting>(builder.Configuration.GetSection("AISetting"));
builder.Services.Configure<CalendarSetting>(builder.Configuration.GetSection("CalendarSetting"));

builder.Services.AddDbContext<DocumentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


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

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler(options => { });
app.UseHttpsRedirection();
app.MapCarter();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
