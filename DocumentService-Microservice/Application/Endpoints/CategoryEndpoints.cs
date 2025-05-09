// using Application.Common.Models.CategoryModel;
// using Application.Common.Ultils;
// using Application.Features.CategoryFeature.Commands;
// using Application.Features.CategoryFeature.Queries;
// using Carter;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Application.Endpoints;
//
// public class CategoryEndpoints : ICarterModule
// {
//     public void AddRoutes(IEndpointRouteBuilder app)
//     {
//         var group = app.MapGroup("api/v1");
//         group.MapGet("categories", GetCategory).WithName(nameof(GetCategory));
//         group.MapPost("category", CreateCategory).RequireAuthorization("moderatorPolicy")
//             .WithName(nameof(CreateCategory));
//         group.MapDelete("category/{id}", DeleteCategory).RequireAuthorization("moderatorPolicy")
//             .WithName(nameof(DeleteCategory));
//         group.MapPatch("category/{id}", UpdateCategory).RequireAuthorization("moderatorPolicy")
//             .WithName(nameof(UpdateCategory));
//
//     }
//
//     public static async Task<IResult> GetCategory(ISender sender, CancellationToken cancellationToken)
//     {
//         var query = new CategoryQuery()
//         {
//
//         };
//         var result = await sender.Send(query, cancellationToken);
//
//         return JsonHelper.Json(result);
//     }
//     public static async Task<IResult> CreateCategory([FromBody]CategoryCreateRequestModel categoryCreateRequestModel,
//         ISender sender, CancellationToken cancellationToken)
//     {
//         var command = new CategoryCreateCommand()
//         {
//             Category = categoryCreateRequestModel,
//         };
//         var result = await sender.Send(command, cancellationToken);
// 		return Results.Json(result, statusCode: (int)result.Status);
// 	}
//     public static async Task<IResult> UpdateCategory([FromBody]CategoryCreateRequestModel categoryCreateRequestModel,
//         ISender sender, CancellationToken cancellationToken, Guid id)
//     {
//         var command = new CategoryUpdateCommand()
//         {
//             Category = categoryCreateRequestModel,
//             Id = id
//         };
//         var result = await sender.Send(command, cancellationToken);
// 		return Results.Json(result, statusCode: (int)result.Status);
// 	}
//     public static async Task<IResult> DeleteCategory(ISender sender, CancellationToken cancellationToken, Guid id)
//     {
//         var command = new CategoryDeleteCommand()
//         {
//             Id = id
//         };
//         var result = await sender.Send(command, cancellationToken);
// 		return Results.Json(result, statusCode: (int)result.Status);
// 	}
// }