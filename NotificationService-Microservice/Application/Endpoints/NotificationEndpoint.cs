using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.SystemNotification;
using Application.Common.Ultils;
using Application.Constants;
using Application.Features.SubcriberFeature;
using Application.Features.SystemNotification;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Novu.DTO.Events;
using SharedProjects.ConsumeModel;
using SharedProjects.ConsumeModel.NovuModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Endpoints
{
    public class NotificationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/notification");
            group.MapPost("subcriber", CreateSubcriber).RequireAuthorization().WithName(nameof(CreateSubcriber));
            group.MapGet("test", TestNotification).WithName(nameof(TestNotification));
        }
       
        public async Task<IResult> CreateSubcriber([FromBody] CreateSubscriberCommand command, ISender sender,
                                                                CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            if (result.Status == System.Net.HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        public async Task<IResult> TestNotification([FromServices] INovuService novuService)
        {
            bool result = await novuService.TestNotification("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            if (result)
            {
                return Results.Ok();
            }

            else return Results.BadRequest();
        }
    }
}
