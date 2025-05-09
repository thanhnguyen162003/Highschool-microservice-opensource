using Application.Common.Models.SubjectModel;
using Application.Common.Ultils;
using Application.Features.SubjectFeature.Commands.V2;
using Carter;
using Domain.CustomModel;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints.V2
{
	public class SubjectEndpoint : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			var group = app.MapGroup("api/v2");
			group.MapPatch("subject", UpdateSubjectV2).RequireAuthorization("moderatorPolicy").WithName(nameof(UpdateSubjectV2)).DisableAntiforgery();
			group.MapPost("subject", CreateSubjectV2).DisableAntiforgery().WithName(nameof(CreateSubjectV2));
		}

		/// <summary>
		/// Updates the details of a subject.
		/// </summary>
		/// <param name="subjectUpdateModelRequest">The request model containing updated subject details.</param>
		/// <param name="sender">The sender for the command.</param>
		/// <param name="mapper">Mapper to map input to domain models.</param>
		/// <param name="validationHelper">Validation helper for input validation.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public static async Task<IResult> UpdateSubjectV2([FromForm] SubjectUpdateModelRequest subjectUpdateModelRequest, ISender sender,
			IMapper mapper, ValidationHelper<SubjectModel> validationHelper, CancellationToken cancellationToken)
		{
			var mapSubject = mapper.Map<SubjectModel>(subjectUpdateModelRequest);
			var (isValid, response) = await validationHelper.ValidateAsync(mapSubject);
			if (!isValid)
			{
				return Results.BadRequest(response);
			}
			var command = new UpdateSubjectCommand()
			{
				SubjectModel = mapSubject
			};
			var result = await sender.Send(command, cancellationToken);
			return Results.Json(result, statusCode: (int)result.Status);
		}

		/// <summary>
		/// Creates a new subject.
		/// </summary>
		/// <param name="subjectCreateRequestModel">The request model containing subject details.</param>
		/// <param name="sender">The sender for the command.</param>
		/// <param name="mapper">Mapper to map input to domain models.</param>
		/// <param name="validationHelper">Validation helper for input validation.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public static async Task<IResult> CreateSubjectV2([FromForm] SubjectCreateRequestModel subjectCreateRequestModel, ISender sender,
			IMapper mapper, ValidationHelper<SubjectCreateRequestModel> validationHelper, CancellationToken cancellationToken)
		{
			var (isValid, response) = await validationHelper.ValidateAsync(subjectCreateRequestModel);
			if (!isValid)
			{
				return Results.BadRequest(response);
			}
			var command = new CreateSubjectCommand()
			{
				SubjectCreateModel = subjectCreateRequestModel
			};
			var result = await sender.Send(command, cancellationToken);
			return Results.Json(result, statusCode: (int)result.Status!);
		}
	}
}
