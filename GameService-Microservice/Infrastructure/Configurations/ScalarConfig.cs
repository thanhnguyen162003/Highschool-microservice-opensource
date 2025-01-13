using Microsoft.AspNetCore.Builder;
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
            });

            app.MapScalarApiReference(options =>
            {
                options.EndpointPathPrefix = "/api/{documentName}";
            })
               .RequireAuthorization(options =>
               {
                   options.RequireAssertion(context =>
                   {
                       return true;
                   });
               });
        }
    }
}
