using Application.Common.Models.ContainerModel;
using Application.Common.Ultils; 
using Application.Features.ContainerFeature.Commands;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints
{
	public class ContainerEndpoints : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			var group = app.MapGroup("api/v1");
			group.MapPatch("container/flashcard/{id}", UpdateContainer).RequireAuthorization().WithName(nameof(UpdateContainer));
            group.MapPatch("container/flashcard/{flashcardId}/preset/{presetId}", ChoosePreset).RequireAuthorization().WithName(nameof(ChoosePreset));
        }

		public static async Task<IResult> UpdateContainer([FromBody]ContainerUpdateRequestModel containerUpdateRequestModel,Guid id, ISender sender, CancellationToken cancellationToken)
		{
			var command = new UpdateContainerCommand()
			{
				ContainerUpdateRequestModel = containerUpdateRequestModel,
				FlashcardId = id
			};
			var result = await sender.Send(command, cancellationToken);
			return Results.Json(result, statusCode: (int)result.Status);
		}

        public static async Task<IResult> ChoosePreset(Guid flashcardId, Guid presetId, ISender sender, CancellationToken cancellationToken)
        {
            var command = new ChooseFSRSPresetCommand()
            {
                FSRSPresetId = presetId,
                FlashcardId = flashcardId
            };
            var result = await sender.Send(command, cancellationToken);
            return Results.Json(result, statusCode: (int)result.Status);
        }

    }
}
