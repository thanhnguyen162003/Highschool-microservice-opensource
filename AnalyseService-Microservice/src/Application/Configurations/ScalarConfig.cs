using Scalar.AspNetCore;

namespace Application.Configurations;

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
            options.Theme = ScalarTheme.DeepSpace;
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
