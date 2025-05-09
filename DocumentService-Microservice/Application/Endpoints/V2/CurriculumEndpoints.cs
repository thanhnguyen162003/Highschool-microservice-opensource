using Application.Common.Models.CurriculumModel;
using Application.Common.Ultils;
using Application.Features.CurrilumFeature.Commands.V2;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints.V2;

public class CurriculumEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v2");
        //group.MapPost("curriculum", CreateCurriculumV2).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateCurriculumV2));
		group.MapPatch("curriculum", UpdateCurriculumV2).RequireAuthorization("moderatorPolicy").WithName(nameof(UpdateCurriculumV2));
	}

 //   public static async Task<IResult> CreateCurriculumV2([FromBody] CurriculumCreateRequestModel curriculumCreateRequestModel,
 //       ISender sender, CancellationToken cancellationToken, ValidationHelper<CurriculumCreateRequestModel> validationHelper)
 //   {
 //       var (isValid, response) = await validationHelper.ValidateAsync(curriculumCreateRequestModel);
 //       if (!isValid)
 //       {
 //           return Results.BadRequest(response);
 //       }
 //       var command = new CreateCurrilumCommand()
 //       {
 //           CurriculumCreateRequestModel = curriculumCreateRequestModel
 //       };
 //       var result = await sender.Send(command, cancellationToken);
	//	return Results.Json(result, statusCode: (int)result.Status);
	//}
	public static async Task<IResult> UpdateCurriculumV2([FromBody] CurriculumUpdateRequestModel curriculumUpdateRequestModel,
		Guid id, ISender sender, CancellationToken cancellationToken, ValidationHelper<CurriculumUpdateRequestModel> validationHelper)
	{
		var (isValid, response) = await validationHelper.ValidateAsync(curriculumUpdateRequestModel);
		if (!isValid)
		{
			return Results.BadRequest(response);
		}
		var command = new CurriculumUpdateCommand()
		{
			CurriculumUpdateRequestModel = curriculumUpdateRequestModel,
			CurriculumId = id
		};
		var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
}