using Application.Common.Models.UserModel;
using Application.Features.Authen.v1.CheckUserName;
using Application.Features.GetCreatorFlashcard;
using Application.Features.RoadmapUser.GetRoadmap;
using Application.Features.User.CreateAccount;
using Application.Features.User.GetAllUser;
using Application.Features.User.GetAuthor;
using Application.Features.User.GetInforUser;
using Application.Features.User.GetPersonalityTestStatus;
using Application.Features.User.GetStudentInfo;
using Application.Features.User.GetProgressStage;
using Application.Features.User.GetUserMbti;
using Application.Features.User.UpdateBaseUser;
using Application.Features.User.UpdateNewUser;
using Application.Features.User.UpdateStatusUser;
using Application.Features.User.UpdateStudent;
using Application.Features.User.UpdateTeacher;
using Carter;
using Domain.Entities;
using Domain.Enumerations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Application.Common.Models.PersonalityResult;
using Application.Services;
using Application.Features.User.Statistic;
using Application.Features.User.UserCurriculum.Command;

namespace Application.Endpoints.v1
{
    public class UserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/users");
            group.MapGet("statistic", GetStatistic).RequireAuthorization().WithName(nameof(GetStatistic));
            group.MapGet("userActivity", GetUserActivity).RequireAuthorization().WithName(nameof(GetUserActivity));
            group.MapGet("userGrowth", GetUserGrowth).RequireAuthorization().WithName(nameof(GetUserGrowth));
            group.MapGet("teacherExperience", GetTeacherExperienceCount).WithName(nameof(GetTeacherExperienceCount));
            group.MapPut("baseuser", UpdateBaseUser).RequireAuthorization().WithName(nameof(UpdateBaseUser)).DisableAntiforgery();
            group.MapGet("roles", GetRoles).WithName(nameof(GetRoles));
            group.MapPut("newuser/{userId}", UpdateNewUser).WithName(nameof(UpdateNewUser));
            group.MapGet("checkusername", CheckUserName).RequireAuthorization().WithName(nameof(CheckUserName));
            group.MapPut("student", UpdateStudent).RequireAuthorization().WithName(nameof(UpdateStudent));
            group.MapGet("student/mbti/mbtiType", GetUserMbtiType).RequireAuthorization().WithName(nameof(GetUserMbtiType));
            group.MapPut("teacher", UpdateTeacher).RequireAuthorization().WithName(nameof(UpdateTeacher));
            group.MapGet("infor/{username}", GetInfoUser).WithName(nameof(GetInfoUser));
            group.MapGet("", GetAllUsers).WithName(nameof(GetAllUsers));
            group.MapGet("author/{userId}", GetAuthorFlashcard).WithName(nameof(GetAuthorFlashcard));
            group.MapPut("status", UpdateStatusUser).RequireAuthorization().WithName(nameof(UpdateStatusUser));
            group.MapPost("createaccount", CreateAccount).RequireAuthorization().WithName(nameof(CreateAccount));
            group.MapGet("roadmap", GetRoadmapUser).RequireAuthorization().WithName(nameof(GetRoadmapUser));
            group.MapGet("personalityTestStatus", GetPersonalityTestStatus).RequireAuthorization("studentPolicy").WithName(nameof(GetPersonalityTestStatus));
            group.MapGet("brief", GetBrief).WithName(nameof(GetBrief));
            group.MapGet("author", GetAuthor).WithName(nameof(GetAuthor));
            group.MapGet("self", GetStudentInfo).RequireAuthorization("studentPolicy").WithName(nameof(GetStudentInfo));
            group.MapGet("progressStage", GetProgressStage).RequireAuthorization().WithName(nameof(GetProgressStage));
            group.MapPost("bio", GenrateBio).WithName(nameof(GenrateBio));
			group.MapPost("subject-curriculum", InsertSubjectCurriculum).RequireAuthorization().WithName(nameof(InsertSubjectCurriculum));
		}
        private static async Task<IResult> GetUserMbtiType(ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetUserMbtiTypeCommand();
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        }
        private static async Task<IResult> UpdateBaseUser(UpdateBaseUserCommand updateBaseUserCommand, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(updateBaseUserCommand, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        private static IResult GetRoles()
        {
            List<Role> colorList = Enum.GetValues(typeof(RoleEnum))
                .Cast<RoleEnum>()
                .Select(c => new Role { Id = (int)c, RoleName = c.ToString() })
                .ToList();

            return Results.Ok(colorList);
        }

        private static async Task<IResult> UpdateNewUser(Guid userId, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateNewUserCommand()
            {
                UserId = userId
            }, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        private static async Task<IResult> CheckUserName([AsParameters] CheckUserNameQuery checkUserNameQuery, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(checkUserNameQuery, cancellationToken);

            return Results.Ok(result);
        }

        private static async Task<IResult> UpdateStudent(UpdateStudentCommand updateStudentCommand, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(updateStudentCommand, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        private static async Task<IResult> UpdateTeacher(UpdateTeacherCommand updateTeacherCommand, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(updateTeacherCommand, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        private static async Task<IResult> GetInfoUser(string username, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetInforUserCommand()
            {
                UserName = username
            }, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);

        }

        private static async Task<IResult> GetAllUsers([AsParameters] GetAllUserQuery getAllUserQuery, ISender sender,
            CancellationToken cancellationToken, HttpContext httpContext)
        {
            getAllUserQuery.Status = httpContext.Request.Query["status"];
            var (response, metadata) = await sender.Send(getAllUserQuery, cancellationToken);

            httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Results.Ok(response);
        }

        private static async Task<IResult> GetAuthorFlashcard(Guid userId, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetCreatorFlashcardQuery()
            {
                UserId = userId
            }, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);

        }

        private static async Task<IResult> UpdateStatusUser(UpdateStatusUserCommand updateStatusUserCommand, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(updateStatusUserCommand, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        private static async Task<IResult> CreateAccount(CreateAccountCommand createAccountCommand, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(createAccountCommand, cancellationToken);

            if (result.Status == HttpStatusCode.Created)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }
        private static async Task<IResult> GetStatistic(string UserType, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetUserAmountCommand()
            {
                UserType = UserType
            }, cancellationToken);
            return Results.Ok(result);
        }
        private static async Task<IResult> GetUserActivity(string UserActivityType, int amount, bool IsCountFromNow, ISender sender, CancellationToken cancellationToken)
        {
            if (amount < 1 && IsCountFromNow)
            {
                return Results.BadRequest("Amount must be at least 1.");
            }
            if (amount < 0 && IsCountFromNow == false)
            {
                return Results.BadRequest("Amount must not be negative.");
            }
            var result = await sender.Send(new GetUserActivityCommand()
            {
                UserActivityType = UserActivityType,
                IsCountFrom = IsCountFromNow,
                Amount = amount
            }, cancellationToken);
            return Results.Ok(result);
        }
        private static async Task<IResult> GetUserGrowth(string UserActivityType, int amount, bool IsCountFromNow, ISender sender, CancellationToken cancellationToken)
        {
            if (amount < 1 && IsCountFromNow)
            {
                return Results.BadRequest("Amount must be at least 1.");
            }
            if (amount < 0 && IsCountFromNow == false)
            {
                return Results.BadRequest("Amount must not be negative.");
            }
            var result = await sender.Send(new GetUserGrowthCommand()
            {
                UserActivityType = UserActivityType,
                IsCountFrom = IsCountFromNow,
                Amount = amount
            }, cancellationToken);
            return Results.Ok(result);
        }
        private static async Task<IResult> GetTeacherExperienceCount(ISender sender, CancellationToken cancellationToken)
        {

            var result = await sender.Send(new GetTeacherExperienceCommand()
            {
            }, cancellationToken);
            return Results.Ok(result);
        }
        private static async Task<IResult> GetRoadmapUser(ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetRoadmapCommand(), cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

        private static async Task<IResult> GetPersonalityTestStatus(ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetPersonalityTestStatusQuery(), cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }

      

        private static async Task<IResult> GetBrief(ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetBriefQuery(), cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(result);
        }
        private static async Task<IResult> GetAuthor([AsParameters] AuthorRequest authorRequest, HttpContext httpContext, ISender sender, CancellationToken cancellationToken)
        {
            var queries = httpContext.Request.Query.SelectMany(x => x.Value).ToList();
            var result = await sender.Send(new GetAuthorCommand()
            {
                UserIds = queries
            }, cancellationToken);

            return Results.Ok(result);
        }

        private static async Task<IResult> GetStudentInfo(ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetStudentInfoQuery(), cancellationToken);
            if (result.Status == HttpStatusCode.NotFound)
            {
                return Results.NotFound(result);
            }
            return Results.Ok(result);
        }
        private static async Task<IResult> GetProgressStage([AsParameters] GetProgressStageQuery getProgressStageQuery, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(getProgressStageQuery, cancellationToken);

            if(result.Status != HttpStatusCode.OK)
            {
                return Results.BadRequest(result);
            }

            return Results.Ok(result);
        }

        

        private static async Task<IResult> GenrateBio([FromBody] DateTime birthDate, IOpenAIService openAIService)
        {
            var result = await openAIService.GenerateBio(birthDate);

            return Results.Ok(result);
        }
		private static async Task<IResult> InsertSubjectCurriculum([FromBody] UserCurriculumCreateCommand userCurriculumCreateCommand,
            ISender sender, CancellationToken cancellation)
		{
            var result = await sender.Send(userCurriculumCreateCommand, cancellation);
            if (result.Status == HttpStatusCode.BadRequest) { return Results.BadRequest(result); }
			if (result.Status == HttpStatusCode.NotFound) { return Results.NotFound(result); }
			return Results.Ok(result);
		}

	}
}
