using Application.Common.Models.SearchModel;
using Application.Features.SearchFeature.commands;
using Application.Services.SearchService;
using Carter;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Application.Endpoints
{
    public class SearchEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/search");
            group.MapPost("", CreateIndex).WithName(nameof(CreateIndex));
        }

        private static async Task<IResult> CreateIndex(SearchCommand searchCommand, ISender sender)
        {
            var result = await sender.Send(searchCommand);

            return result.Status == HttpStatusCode.OK ? Results.Ok(result) : Results.BadRequest(result);
        }

    }
}
