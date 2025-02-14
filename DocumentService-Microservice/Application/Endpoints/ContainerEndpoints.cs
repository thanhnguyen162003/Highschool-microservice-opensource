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
			group.MapPost("container", UpdateContainer).RequireAuthorization().WithName(nameof(UpdateContainer));
		}

		public static async Task<IResult> UpdateContainer([FromBody]ContainerUpdateRequestModel containerUpdateRequestModel, ISender sender,
			ValidationHelper<ContainerUpdateRequestModel> validationHelper, CancellationToken cancellationToken)
		{
			var command = new UpdateContainerCommand()
			{
				ContainerUpdateRequestModel = containerUpdateRequestModel
			};
			var result = await sender.Send(command, cancellationToken);
			return Results.Json(result, statusCode: (int)result.Status);
		}
		
	}
}
