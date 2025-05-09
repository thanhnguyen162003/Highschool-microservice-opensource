using Application.Common.Models.OccupationModel;
using Application.Features.Occupation.Commands;
using Application.Features.Occupation.Queries;
using Carter;
using Domain.Common;
using Infrastructure.QueryFilters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Application.Endpoints.v1
{
    public class OccupationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/occupation");
            group.MapPost("", CreateOccupation).WithName(nameof(CreateOccupation));
            group.MapPut("{id}", UpdateOccupation).WithName(nameof(UpdateOccupation));
            group.MapGet("", GetOccupations).WithName(nameof(GetOccupations));
            group.MapDelete("{id}", DeleteOccupationById).WithName(nameof(DeleteOccupationById));

            //group.MapGet("{occupationId}", GetOccupationById).WithName(nameof(GetOccupationById));
            //group.MapGet("", GetOccupations).WithName(nameof(GetOccupations));
        }

        public static async Task<IResult> CreateOccupation([FromBody] List<OccupationRequestModel> occupationRequestModelList,
    ISender sender, CancellationToken cancellationToken)
        {
            var command = new OccupationCreateCommand
            {
                OccupationRequestModelList = occupationRequestModelList
            };

            var result = await sender.Send(command, cancellationToken);

            if (result.Status == HttpStatusCode.Created)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        public static async Task<IResult> UpdateOccupation(
    [Required] string id,
    [FromBody] OccupationRequestModel request,
    ISender sender, CancellationToken cancellationToken)
        {
            var command = new OccupationUpdateCommand
            {
                Id = id,
                Request = request
            };

            var result = await sender.Send(command, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }



        //public static async Task<IResult> GetOccupationById([Required] string occupationId, ISender sender, CancellationToken cancellationToken)
        //{
        //    var command = new GetOccupationById()
        //    {
        //        OccupationId = occupationId
        //    };

        //    var result = await sender.Send(command, cancellationToken);
        //    if (result.Status == System.Net.HttpStatusCode.OK)
        //    {
        //        return Results.Ok(result);
        //    }

        //    return Results.BadRequest(result);
        //}

        public static async Task<IResult> GetOccupations(
    [AsParameters] OccupationQueryFilter filter,
    ISender sender,
    CancellationToken cancellationToken,
    HttpContext httpContext)
        {
            var query = new GetOccupations
            {
                QueryFilter = filter,
            };

            var result = await sender.Send(query, cancellationToken);

            var metadata = new Metadata
            {
                TotalCount = result.TotalItems,
                PageSize = result.EachPage,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages
            };

            httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Results.Ok(result);
        }
        public static async Task<IResult> DeleteOccupationById([Required] string id, ISender sender, CancellationToken cancellationToken)
        {
            var command = new OccupationDeleteCommand
            {
                Id = id
            };

            var result = await sender.Send(command, cancellationToken);

            if (!result)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }
    }
}
