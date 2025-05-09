using Carter;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Dapr.Users;
using Application.Features.Dapr.Academic;
using Application.Common.Models.DaprModel.User;
using Application.Features.Dapr.User;

namespace Application.Endpoints.v1
{
    public class DaprEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/dapr");
            group.MapGet("user-login-count", GetUserLoginCount).WithName(nameof(GetUserLoginCount));
            group.MapGet("user-academic", GetUserForAcademic).WithName(nameof(GetUserForAcademic));
            group.MapGet("user-count", GetCountUser).WithName(nameof(GetCountUser));
            group.MapGet("user-media", GetUserForMedia).WithName(nameof(GetUserForMedia));
            group.MapGet("user", GetUser).WithName(nameof(GetUser));
			group.MapGet("subject-curriculum-user/subject/{subjectId}/user/{userId}", GetSubjectCurriculumUserDapr).WithName(nameof(GetSubjectCurriculumUserDapr));

		}
		public static async Task<IResult> GetSubjectCurriculumUserDapr(Guid subjectId, Guid userId,
			 ISender sender, CancellationToken cancellationToken)
		{
			var query = new DaprGetSubjectCurriculumUser()
			{
				SubjectId = subjectId,
                UserId = userId,
			};
			var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
		}
		public static async Task<IResult> GetUserLoginCount(ISender sender, CancellationToken cancellationToken)
        {
            var query = new DaprGetUserLoginCount();
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        }
        public static async Task<IResult> GetUserForAcademic([FromQuery] string[] email, ISender sender, CancellationToken cancellationToken)
        {
            var query = new DaprGetUsersAcademic()
            {
                Email = email
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
        public static async Task<IResult> GetCountUser([AsParameters] UserCountRequestDapr userCountRequest, ISender sender, CancellationToken cancellationToken)
        {
            var query = new DaprGetCountUser()
            {
                UserCount = userCountRequest
            };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        }

        public static async Task<IResult> GetUserForMedia(Guid? userId, ISender sender, CancellationToken cancellationToken)
        {
            var query = new DaprGetUserMedia()
            {
                UserId = userId.ToString()
            };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        }
        public static async Task<IResult> GetUser(string username, ISender sender, CancellationToken cancellationToken)
        {
            var query = new DaprGetUsers()
            {
                Username = username
            };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        }
    }
}
