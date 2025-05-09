// using Carter;
// using System.ComponentModel.DataAnnotations;
// using System.Net;

// namespace Application.Endpoints.v1
// {
// 	public class StudentEndpoints : ICarterModule
// 	{
// 		public void AddRoutes(IEndpointRouteBuilder app)
// 		{
// 			var group = app.MapGroup("api/v1/student");
// 			group.MapPatch("{majorId}", AddMajorStudent)
// 				.RequireAuthorization()
// 				.WithName(nameof(AddMajorStudent));
// 		}

// 		public static async Task<IResult> AddMajorStudent(
// 			[Required] string majorId,
// 			ISender sender,
// 			CancellationToken cancellationToken)
// 		{
// 			var command = new MajorUserFollowCommand
// 			{
// 				MajorId = majorId
// 			};
// 			var result = await sender.Send(command, cancellationToken);

// 			return result.Status switch
// 			{
// 				HttpStatusCode.OK => Results.Ok(result),
// 				HttpStatusCode.BadRequest => Results.BadRequest(result),
// 				HttpStatusCode.Unauthorized => Results.Unauthorized(),
// 				HttpStatusCode.NotFound => Results.NotFound(result),
// 				HttpStatusCode.InternalServerError => Results.StatusCode((int)HttpStatusCode.InternalServerError),
// 				_ => Results.StatusCode((int)(result.Status ?? HttpStatusCode.InternalServerError))
// 			};
// 		}
// 	}
// }