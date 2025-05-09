using Application.Features.QuestionBank.Holland.Queries;
using Application.Features.QuestionBank.Holland;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Application.Common.Models.HollandModel;
using Application.Features.PersonalityTest.QuestionBank.Holland.Queries;

namespace Application.Endpoints.v1
{
    public class HollandEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/holland");

            group.MapGet("", GetHollandTest).WithName(nameof(GetHollandTest));
            group.MapGet("all", GetAllHollandTypeContent).WithName(nameof(GetAllHollandTypeContent));
            group.MapPost("", GetResultHolland).WithName(nameof(GetResultHolland));
        }

        public static async Task<IResult> GetHollandTest(ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetHollandTestQuery();
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        }

        public static async Task<IResult> GetResultHolland([FromBody] List<HollandResultRequestModel> list, ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetHollandResultQuery()
            {
                AnswerList = list,
            };
            try
            {
                var result = await sender.Send(query, cancellationToken);
                return Results.Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return Results.BadRequest();
            }
        }

        public static async Task<IResult> GetAllHollandTypeContent(ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetAllHollandTypeContent();
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        }
    }
}
