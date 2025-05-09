using Application.Features.SubjectCurriculumFeature.Commands.V2;
using Carter;

namespace Application.Endpoints.V2;

public class SubjectCurriculumEndpoints : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("api/v2");
		group.MapPatch("subject/{subjectId}/curriculum/{curriculumId}/publish", PublishSubjectV2).RequireAuthorization("moderatorPolicy").WithName(nameof(PublishSubjectV2));
		group.MapPatch("subject/{subjectId}/curriculum/{curriculumId}/unpublish", UnPublishSubjectV2).RequireAuthorization("moderatorPolicy").WithName(nameof(UnPublishSubjectV2));
	}
	public static async Task<IResult> PublishSubjectV2(Guid subjectId, Guid curriculumId, ISender sender, CancellationToken cancellationToken)
	{
		var command = new PublishSubjectCurriculumCommand()
		{
			SubjectId = subjectId,
			CurriculumId = curriculumId
		};
		var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
	public static async Task<IResult> UnPublishSubjectV2(Guid subjectId, Guid curriculumId, ISender sender, CancellationToken cancellationToken)
	{
		var command = new UnPublishSubjectCurriculumCommand()
		{
			SubjectId = subjectId,
			CurriculumId = curriculumId
		};
		var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
}
