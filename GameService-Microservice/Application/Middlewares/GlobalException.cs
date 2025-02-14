using Application.Messages;
using Domain.Models.Common;
using FluentValidation;
using System.Net;

namespace Application.Middlewares
{
    public class GlobalException : IMiddleware
    {

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

        private Task HandleValidationException(HttpContext context, ValidationException ex)
        {
            var errors = ex.Errors.Select(x => new ErrorValidation()
            {
                PropertyName = x.PropertyName,
                ErrorMessage = x.ErrorMessage
            }).ToArray();
            int statusCode = (int)HttpStatusCode.BadRequest;
            var errorResponse = new APIResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Message = MessageCommon.SomethingErrors,
                Data = errors
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(errorResponse.ToString()!);
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            var methodError = ex.TargetSite?.DeclaringType?.FullName;
            var errorResponse = new ErrorResponse()
            {
                StatusCode = statusCode,
                Message = ex.GetType().ToString(),
                Location = (methodError != null ? ("Class: " + methodError + ", ") : "") + "Method: " + ex.TargetSite?.Name,
                Detail = ex.Message,
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(errorResponse.ToString());
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
