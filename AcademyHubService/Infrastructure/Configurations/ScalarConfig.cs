using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace Infrastructure.Configurations
{
    public static class ScalarConfig
    {
        public static void UseScalar(this WebApplication app)
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "/openapi/{documentName}.json";
                options.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers.Add(new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" });
                });
            });

            app.MapScalarApiReference(options =>
            {
                options.EndpointPathPrefix = "/api/{documentName}";
            });
        }
    }
}
