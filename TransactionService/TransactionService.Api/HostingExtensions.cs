using TransactionService.Api.Impl;
using TransactionService.ReadModel.Queries.RavenDB;
using TransactionService.ReadModel;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using TransactionService.Api.ServiceInterface;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using TransactionService.Api.Impl.AutoMapperUtils;

namespace TransactionService.Api;

public static class HostingExtensions
{
    const string CorsPolicyName = "CorsPolicy";
    const string ApiScopePolicyName = "ApiScope";
    const string NpgCnnKey = "PostgreSQL:ConnectionString";

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ITimeProvider, StarnetTimeProvider>();
        var store = new RavenDocumentStoreFactory().CreateAndInitializeDocumentStore(
            RavenConfig.FromConfiguration(builder
                .Configuration)); // leave it here to avoid lazy loading until this is refactored so that this comment is NOT NEEDED
        builder.Services.AddSingleton(store);
        builder.Services.AddTransient<ITypeaheadQueries, TypeaheadQueries>();
        builder.Services.AddTransient<IOrganizationQueries, OrganizationQueries>();
        builder.Services.AddTransient<IQueryById, QueryById>();
        builder.Services.AddAutoMapper(typeof(CommandsProfile).Assembly);


        var a = typeof(OrganizationQueryController).Assembly;
        builder.Services.AddControllers()
            .AddApplicationPart(a)
            .AddControllersAsServices();

        builder.Services.AddValidatorsFromAssemblyContaining<RegisterOrganizationValidator>()
            .AddFluentValidationAutoValidation(cfg => { cfg.DisableBuiltInModelValidation = true; });

        //AddAuthenticationAndAuthorizationWithBearerAndCookieSupport(builder);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Host.UseNServiceBus((ctx) => EndpointConfigurationFactory.Create(ctx.Configuration));
        return builder.Build();
    }

    static void AddAuthenticationAndAuthorizationWithBearerAndCookieSupport(WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication("Bearer")
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = builder.Configuration["IdentityServer:Url"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "api");
            });
        });
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        // app.UseAuthentication();

        // app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); //.RequireAuthorization("ApiScope");
        });

        return app;
    }
}