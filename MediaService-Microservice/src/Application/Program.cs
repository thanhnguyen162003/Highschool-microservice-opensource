using System.Text;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Application;
using Application.Common.Exceptions;
using Application.Common.Models.AIModels;
using Application.Common.Ultils;
using Application.Configuration;
using Azure.Storage.Blobs;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SignalR;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddWebServices();
builder.Services.AddControllers();
builder.Services.AddCarter();
builder.Services.AddDaprClient();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = long.MaxValue;
});

builder.Services.AddRedis(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition", "Location", "X-Pagination");
        });
});
builder.Services.AddSingleton(x =>
{
    var configuration = x.GetRequiredService<IConfiguration>();
    var connectionString = configuration["Azure:ConnectionString"];
    return new BlobServiceClient(connectionString);
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);
builder.Services.AddDistributedMemoryCache();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.AllowAnyClientCertificate();
    });
});
//grpc
//builder.Services
//    .AddGrpcClient<SubjectServiceCheckRpc.SubjectServiceCheckRpcClient>((services, options) =>
//    {
//        options.Address = new Uri(builder.Configuration["GRPC:DocumentService"]);
//    });
//builder.Services
//    .AddGrpcClient<LessonServiceCheckRpc.LessonServiceCheckRpcClient>((services, options) =>
//    {
//        options.Address = new Uri(builder.Configuration["GRPC:DocumentService"]);
//    });

//builder.Services
//    .AddGrpcClient<DocumentServiceRpc.DocumentServiceRpcClient>((services, options) =>
//    {
//        options.Address = new Uri(builder.Configuration["GRPC:DocumentService"]);
//    });
//builder.Services
//    .AddGrpcClient<FlashcardServiceRpc.FlashcardServiceRpcClient>((services, options) =>
//    {
//        options.Address = new Uri(builder.Configuration["GRPC:DocumentService"]);
//    });
//builder.Services
//    .AddGrpcClient<TheoryServiceRpc.TheoryServiceRpcClient>((services, options) =>
//    {
//        options.Address = new Uri(builder.Configuration["GRPC:DocumentService"]);
//    });

//builder.Services
//    .AddGrpcClient<UserServiceRpc.UserServiceRpcClient>((services, options) =>
//    {
//        options.Address = new Uri(builder.Configuration["GRPC:UserService"]);
//    });

//builder.Services.AddGrpc();

//
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.Configure<DocumentAISetting>(builder.Configuration.GetSection("DocumentAISetting"));
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
    options.AddPolicy("AdminOrModeratorPolicy", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("Role", "1", "2"));
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
// Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseExceptionHandler(options => { });
app.UseHttpsRedirection();
app.MapCarter();
app.UseAuthentication();
app.MapControllers();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
// app.UseHealthChecks("/health");
// SignalR
app.MapHub<MyHub>("/hub");
app.Run();
