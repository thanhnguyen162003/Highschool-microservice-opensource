using Application.Common.Models;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.QuestionAnswerModel;
using Application.Common.Models.QuestionModel;
using Application.Common.Ultils;
using Application.Features.QuestionFeature.Commands;
using Application.Features.QuestionFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.Enums;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Endpoints;

public class QuestionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/questions");
        group.MapPost("", CreateQuestions).RequireAuthorization().WithName(nameof(CreateQuestions));
        group.MapDelete("", DeleteQuestions).RequireAuthorization().WithName(nameof(DeleteQuestions));
        group.MapPut("{id}", UpdateQuestion).RequireAuthorization().WithName(nameof(UpdateQuestion));
        group.MapGet("filter", GetQuestionsAdvanceFilter).RequireAuthorization().WithName(nameof(GetQuestionsAdvanceFilter));
        group.MapPost("quiz/submit", SubmitQuizAnswer).RequireAuthorization().WithName(nameof(SubmitQuizAnswer));
        group.MapGet("quiz", GetQuiz).RequireAuthorization().WithName(nameof(GetQuiz));
    }

    public static async Task<IResult> CreateQuestions(
        [FromBody] List<QuestionRequestModel> questionRequestModels,
        ISender sender,
        ValidationHelper<List<QuestionRequestModel>> validationHelper,
        CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(questionRequestModels);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateQuestionCommand()
        {
            Questions = questionRequestModels
        };

        var result = await sender.Send(command, cancellationToken);

        return JsonHelper.Json(result);
    }

    public static async Task<IResult> DeleteQuestions(
    [FromBody] List<Guid> questionIds,
    ISender sender,
    CancellationToken cancellationToken)
    {
        if (questionIds == null || !questionIds.Any())
        {
            return Results.BadRequest(new ResponseModel
            {
                Status = System.Net.HttpStatusCode.BadRequest,
                Message = "Danh sách ID không được để trống."
            });
        }

        var command = new DeleteQuestionsCommand
        {
            QuestionIds = questionIds
        };

        var result = await sender.Send(command, cancellationToken);

        return JsonHelper.Json(result);
    }

    public static async Task<IResult> UpdateQuestion(
    [Required] Guid id,
    [FromBody] QuestionRequestModel requestModel,
    ISender sender,
    ValidationHelper<QuestionRequestModel> validationHelper,
    CancellationToken cancellationToken)
    {
        // Xác thực dữ liệu đầu vào
        var (isValid, response) = await validationHelper.ValidateAsync(requestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }

        var command = new UpdateQuestionCommand()
        {
            QuestionId = id,
            Question = requestModel
        };

        var result = await sender.Send(command, cancellationToken);

        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetQuestionsAdvanceFilter(
    [FromQuery] Guid? lessonId,
    [FromQuery] Guid? chapterId,
    [FromQuery] Guid? subjectCurriculumId,
    [FromQuery] Guid? subjectId,
    [FromQuery] Difficulty? difficulty,
    [FromQuery] QuestionType? questionType,
    [FromQuery] QuestionCategory? category,
    [FromQuery] string? search,
    [FromQuery] int pageSize,
    [FromQuery] int pageNumber,
    ISender sender,
    CancellationToken cancellationToken,
    HttpContext httpContext)
    {
        var queryFilter = new QuestionAdvanceQueryFilter
        {
            LessonId = lessonId,
            ChapterId = chapterId,
            SubjectCurriculumId = subjectCurriculumId,
            SubjectId = subjectId,
            Difficulty = difficulty,
            QuestionType = questionType,
            Category = category,
            Search = search,
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        var query = new QuestionAdvanceQuery
        {
            QueryFilter = queryFilter
        };

        var result = await sender.Send(query, cancellationToken);

        var metadata = new Metadata
        {
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            CurrentPage = result.CurrentPage,
            TotalPages = result.TotalPages
        };

        httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> SubmitQuizAnswer(
    [FromBody] SubmitAnswerRequestModel model,
    ISender sender,
    ValidationHelper<SubmitAnswerRequestModel> validationHelper,
    CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(model);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }

        var command = new SubmitQuizAnswerCommand { RequestModel = model };
        var result = await sender.Send(command, cancellationToken);

        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetQuiz(
    [FromQuery] QuestionCategory questionCategory,
    [FromQuery] Guid categoryId,
    ISender sender,
    ValidationHelper<GetQuizRequestModel> validationHelper,
    CancellationToken cancellationToken)
    {
        // Tạo Request Model từ query parameters
        var model = new GetQuizRequestModel
        {
            QuestionCategory = questionCategory,
            CategoryId = categoryId
        };

        // Xác thực dữ liệu đầu vào
        var (isValid, response) = await validationHelper.ValidateAsync(model);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }

        // Gửi query
        var query = new GetQuizQuery { RequestModel = model };
        var result = await sender.Send(query, cancellationToken);

        return JsonHelper.Json(result);
    }


}