using Application.Common.Models.UniversityModel;
using Application.Features.University.Commands;
using Application.Features.University.Queries;
using Application.Features.UniversityTag.Commands;
using Application.Features.UniversityTag.Queries;
using Application.Features.UniversityTags.Commands;
using Carter;
using Domain.Common;
using Infrastructure.QueryFilters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Application.Endpoints.v1
{
    public class UniversityTagEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/universityTag");
            group.MapPost("", CreateUniversityTag).WithName(nameof(CreateUniversityTag));
            group.MapPut("{id}", UpdateUniversityTag).WithName(nameof(UpdateUniversityTag));
            group.MapGet("", GetUniversityTags).WithName(nameof(GetUniversityTags));
            group.MapDelete("{id}", DeleteUniversityTag).WithName(nameof(DeleteUniversityTag));

            group.MapGet("{id}", GetUniversityTagById).WithName(nameof(GetUniversityTagById));
        }

        public static async Task<IResult> CreateUniversityTag([FromBody] List<string> names, ISender sender, CancellationToken cancellationToken)
        {
            var command = new UniversityTagsCreateCommand
            {
                NameTag = names
            };

            var result = await sender.Send(command, cancellationToken);

            if (result.Status == HttpStatusCode.Created)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }


        public static async Task<IResult> DeleteUniversityTag([Required] Guid id, ISender sender, CancellationToken cancellationToken)
        {
            var command = new UniversityTagsDeleteCommand
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




        public static async Task<IResult> UpdateUniversityTag(
    [Required] Guid id,
    [Required] string name,
    ISender sender, CancellationToken cancellationToken)
        {
            var command = new UniversityTagsUpdateCommand
            {
                Id = id,
                Name = name
            };

            var result = await sender.Send(command, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }


        public static async Task<IResult> GetUniversityTagById([Required] Guid id, ISender sender, CancellationToken cancellationToken)
        {
            var command = new GetUniversityTagById()
            {
                Id = id
            };

            var result = await sender.Send(command, cancellationToken);
            if (result.Status == System.Net.HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        public static async Task<IResult> GetUniversityTags([AsParameters] UniversityTagQueryFilter filter, ISender sender, CancellationToken cancellationToken, HttpContext httpContext)
        {
            var query = new GetUniversitiesTag
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

    }
}
