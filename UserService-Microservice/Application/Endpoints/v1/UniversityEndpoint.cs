using Application.Common.Models.UniversityModel;
using Application.Features.University.Commands;
using Application.Features.University.Queries;
using Carter;
using Domain.Common;
using Infrastructure.QueryFilters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Application.Endpoints.v1
{
    public class UniversityEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/university");
            group.MapPost("", CreateUniversity).WithName(nameof(CreateUniversity));
            //group.MapDelete("", DeleteUniversity).WithName(nameof(DeleteUniversity));
            group.MapPut("{id}", UpdateUniversity).WithName(nameof(UpdateUniversity));
            group.MapGet("", GetUniversities).WithName(nameof(GetUniversities));
            group.MapGet("name", GetUniversitiesNew).WithName(nameof(GetUniversitiesNew));
            group.MapDelete("{id}", DeleteUniversityById).WithName(nameof(DeleteUniversityById));
            group.MapGet("saved", GetSavedUniversities).RequireAuthorization("studentPolicy").WithName(nameof(GetSavedUniversities));
            group.MapPost("saved", SaveUniversity).RequireAuthorization("studentPolicy").WithName(nameof(SaveUniversity));
            group.MapDelete("saved", DeleteSavedUniversity).RequireAuthorization("studentPolicy").WithName(nameof(DeleteSavedUniversity));

            //group.MapGet("{universityId}", GetUniversityById).WithName(nameof(GetUniversityById));
            //group.MapGet("", GetUniversities).WithName(nameof(GetUniversities));
        }

        public static async Task<IResult> CreateUniversity([FromBody] List<UniversityRequestModel> universityRequestModelList, ISender sender, CancellationToken cancellationToken)
        {
            var command = new UniversityCreateCommand
            {
                UniversityRequestModelList = universityRequestModelList
            };

            var result = await sender.Send(command, cancellationToken);

            if (result.Status == HttpStatusCode.Created)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }


        public static async Task<IResult> DeleteUniversityById([Required] Guid id, ISender sender, CancellationToken cancellationToken)
        {
            var command = new UniversityDeleteCommand
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




        public static async Task<IResult> UpdateUniversity(
    [Required] Guid id,
    [FromBody] UniversityRequestModel request,
    ISender sender, CancellationToken cancellationToken)
        {
            var command = new UniversityUpdateCommand
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


        //public static async Task<IResult> GetUniversityById([Required] string universityId, ISender sender, CancellationToken cancellationToken)
        //{
        //    var command = new GetUniversityById()
        //    {
        //        UniversityId = universityId
        //    };

        //    var result = await sender.Send(command, cancellationToken);
        //    if (result.Status == System.Net.HttpStatusCode.OK)
        //    {
        //        return Results.Ok(result);
        //    }

        //    return Results.BadRequest(result);
        //}

        public static async Task<IResult> GetUniversities([AsParameters] UniversityQueryFilter filter, ISender sender, CancellationToken cancellationToken, HttpContext httpContext)
        {
            var query = new GetUniversities
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

        public static async Task<IResult> GetSavedUniversities([AsParameters] GetSavedUniversitiesQuery filter, ISender sender, CancellationToken cancellationToken, HttpContext httpContext)
        {
            var result = await sender.Send(filter, cancellationToken);

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

        public static async Task<IResult> SaveUniversity([FromBody] SaveUniversityCommand command, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        public static async Task<IResult> DeleteSavedUniversity([AsParameters] DeleteSavedUniversityCommand command, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            if (result.Status == HttpStatusCode.NoContent)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        public static async Task<IResult> GetUniversitiesNew([AsParameters] UniversityQueryFilter filter, ISender sender, CancellationToken cancellationToken, HttpContext httpContext)
        {
            var query = new GetUniversitiesName
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
