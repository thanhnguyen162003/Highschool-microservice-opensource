using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Yarp.APIGateway.Middelwares;

var builder = WebApplication.CreateBuilder(args);

//swagger config
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add config for yarp reverse proxy
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//add caching
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("customPolicy", builder => builder.Expire(TimeSpan.FromSeconds(20)));
});

//cors config
builder.Services.AddCors(options =>
{
    options.AddPolicy("customCors", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});
//adding OpenTelemetry for watch monitoring
// builder.Services
//     .AddOpenTelemetry()
//     .ConfigureResource(resource => resource.AddService(serviceName))
//     .WithTracing(tracing =>
//     {
//         tracing
//             .AddAspNetCoreInstrumentation()
//             .AddHttpClientInstrumentation()
//             .AddEntityFrameworkCoreInstrumentation()
//             .AddRedisInstrumentation()
//             .AddNpgsql();
//
//         tracing.AddOtlpExporter();
//     });

//add authentication/authorization
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
    options.AddPolicy("customPolicyAuthentication", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("Role", "1", "2", "3", "4"));
});
//add rate limit
var rateLimiterConfig = builder.Configuration.GetSection("RateLimiter");

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = rateLimiterConfig.GetValue<int>("Fixed:PermitLimit");
        options.Window = TimeSpan.FromSeconds(rateLimiterConfig.GetValue<int>("Fixed:WindowInSeconds"));
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = rateLimiterConfig.GetValue<int>("Fixed:QueueLimit");
    });

    rateLimiterOptions.AddTokenBucketLimiter("burst", options =>
    {
        options.TokenLimit = rateLimiterConfig.GetValue<int>("Burst:TokenLimit");
        options.TokensPerPeriod = rateLimiterConfig.GetValue<int>("Burst:TokensPerPeriod");
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(rateLimiterConfig.GetValue<int>("Burst:ReplenishmentPeriodInSeconds"));
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = rateLimiterConfig.GetValue<int>("Burst:QueueLimit");
    });
});

var app = builder.Build();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseRateLimiter();
app.UseAuthentication();
app.UseCors();
app.UseAuthorization();
app.UseOutputCache();
app.MapReverseProxy();
app.Run();