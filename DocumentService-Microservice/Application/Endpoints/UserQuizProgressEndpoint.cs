using Application.Common.Models.QuestionModel;
using Application.Common.Models.UserQuizProgress;
using Application.Common.Ultils;
using Application.Features.UserQuizProgressFeature.Commands;
using Application.Features.UserQuizProgressFeature.Queries;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints
{
    public class UserQuizProgressEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/quizprogress");
            group.MapGet("status", UserHasQuizProgressQuery).RequireAuthorization().WithName(nameof(UserHasQuizProgressQuery));
            group.MapDelete("", ClearQuizProgress).RequireAuthorization().WithName(nameof(ClearQuizProgress));
        }

        public static async Task<IResult> ClearQuizProgress(
        [FromBody] ClearUserQuizProgressRequestModel request,
        ISender sender,
        ValidationHelper<ClearUserQuizProgressRequestModel> validationHelper,
        CancellationToken cancellationToken)
        {
            var (isValid, response) = await validationHelper.ValidateAsync(request);
            if (!isValid)
            {
                return Results.BadRequest(response);
            }
            var command = new ClearUserQuizProgressCommand()
            {
                Model = request
            };

            var result = await sender.Send(command, cancellationToken);

			return Results.Json(result, statusCode: (int)result.Status);
		}

        public static async Task<IResult> UserHasQuizProgressQuery(
        [FromBody] GetQuizRequestModel request,
        ISender sender,
        ValidationHelper<GetQuizRequestModel> validationHelper,
        CancellationToken cancellationToken)
        {
            var (isValid, response) = await validationHelper.ValidateAsync(request);
            if (!isValid)
            {
                return Results.BadRequest(response);
            }
            var command = new UserHasQuizProgressQuery()
            {
                RequestModel = request
            };

            var result = await sender.Send(command, cancellationToken);

            return JsonHelper.Json(result);
        }
    }
}
