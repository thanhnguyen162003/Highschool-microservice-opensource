using Application.Common.Models.UniversityMajor;
using Application.Features.UniversityMajor.Commands;
using Application.Features.UniversityMajor.Queries;
using Carter;
using Domain.Common;
using Infrastructure.QueryFilters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Application.Endpoints.v1
{
    public class UniversityMajorEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/universityMajor");
            group.MapPost("", CreateUniversityMajor).WithName(nameof(CreateUniversityMajor));
            group.MapPut("{id}", UpdateUniversityMajor).WithName(nameof(UpdateUniversityMajor));
            group.MapGet("", GetUniversityMajors).WithName(nameof(GetUniversityMajors));
            group.MapDelete("{id}", DeleteUniversityMajorById).WithName(nameof(DeleteUniversityMajorById));
        }

        public static async Task<IResult> CreateUniversityMajor([FromBody] List<UniversityMajorRequestModel> universityMajorRequestModelList, ISender sender, CancellationToken cancellationToken)
        {
            var command = new UniversityMajorCreateCommand
            {
                UniversityMajorRequestModelList = universityMajorRequestModelList
            };

            var result = await sender.Send(command, cancellationToken);

            if (result.Status == HttpStatusCode.Created)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        public static async Task<IResult> UpdateUniversityMajor(
    [Required] string id,
    [FromBody] UniversityMajorRequestModel request,
    ISender sender, CancellationToken cancellationToken)
        {
            var command = new UniversityMajorUpdateCommand
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
        public static async Task<IResult> GetUniversityMajors(
    [AsParameters] UniversityMajorQueryFilter filter,
    ISender sender,
    CancellationToken cancellationToken,
    HttpContext httpContext)
        {
            var query = new GetUniversityMajors
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

        public static async Task<IResult> DeleteUniversityMajorById([Required] string id, ISender sender, CancellationToken cancellationToken)
        {
            var command = new UniversityMajorDeleteCommand
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
