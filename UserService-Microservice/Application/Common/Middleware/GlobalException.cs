using Application.Common.Models.Common;
using Domain.Common.Messages;
using System.Net;

namespace FAI.API.Middleware
{
	public class GlobalException : IMiddleware
	{

		public GlobalException()
		{
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next(context);
			} catch (ValidationException ex)
			{
				await HandleValidationException(context, ex);
			} catch (Exception ex)
			{
				await HandleException(context, ex);
			}

		}

		private static Task HandleValidationException(HttpContext context, ValidationException ex)
		{
			var errors = ex.Errors.Select(x => new
			{
				x.PropertyName,
				x.ErrorMessage
			}).DistinctBy(x => x.PropertyName).ToArray();
			int statusCode = (int)HttpStatusCode.BadRequest;
			var errorResponse = new ResponseModel()
			{
				Status = HttpStatusCode.BadRequest,
				Message = MessageCommon.SomethingErrors,
				Data = errors
			};
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = statusCode;

			return context.Response.WriteAsync(errorResponse.ToString()!);
		}

		private static Task HandleException(HttpContext context, Exception ex)
		{
			var statusCode = HttpStatusCode.InternalServerError;
			var errorResponse = new ResponseModel()
			{
				Status = statusCode,
				Message = ex.GetType().ToString(),
				Data = ex.Message,
			};
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)statusCode;

			return context.Response.WriteAsync(errorResponse.ToString()!);
		}
	}

	public static class ExceptionExtention
	{
		public static void ConfigureExceptionHandler(this IApplicationBuilder app)
		{
			app.UseMiddleware<GlobalException>();
		}
	}
}
