using Application.Common.Ultils;
using Application.Features.EnrollmentFeature.Commands;
using Application.Features.EnrollmentProcessFeature.Commands;
using Carter;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Application.Endpoints
{
    public class EnrollmentProgressEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1");
            group.MapPost("enrollProgress/{lessonId}", AddProgress).RequireAuthorization("studentPolicy").WithName(nameof(AddProgress));
        }

        public static async Task<IResult> AddProgress([Required] Guid lessonId,
                                                                ISender sender,
                                                                CancellationToken cancellationToken,
                                                                HttpContext httpContext)
        {
            var query = new AddProgressCommand()
            {
                LessonId = lessonId,
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }
    }
}
