using Application.Common.Ultils;
using Application.Features.DocumentFeature.Queries;
using Application.Features.EnrollmentFeature.Commands;
using Application.Features.EnrollmentFeature.Queries;
using Carter;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Endpoints
{
    public class EnrollmentEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1");
            group.MapGet("top-engage", GetTopEnrollmentStatistic).RequireAuthorization().WithName(nameof(GetTopEnrollmentStatistic));
            group.MapGet("engage-by-type", GetEnrollmentAmount).RequireAuthorization().WithName(nameof(GetEnrollmentAmount));
            group.MapPost("enroll", EnrollSubject).RequireAuthorization("studentPolicy").WithName(nameof(EnrollSubject));
            group.MapDelete("unenroll", UnenrollSubject).RequireAuthorization("studentPolicy").WithName(nameof(UnenrollSubject));
        }

        public static async Task<IResult> EnrollSubject([FromQuery] Guid subjectId, [FromQuery] Guid curriculumId,
                                                                ISender sender,
                                                                CancellationToken cancellationToken,
                                                                HttpContext httpContext)
        {
            var query = new EnrollSubjectCommand()
            {
                SubjectId = subjectId,
                CurriculumId = curriculumId
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }

        public static async Task<IResult> UnenrollSubject([FromQuery] Guid subjectId, [FromQuery] Guid curriculumId,
                                                                ISender sender,
                                                                CancellationToken cancellationToken,
                                                                HttpContext httpContext)
        {
            var query = new UnenrollSubjectCommand()
            {
                SubjectId = subjectId,
                CurriculumId = curriculumId
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }
        public static async Task<IResult> GetTopEnrollmentStatistic(string filter, ISender sender,
                                                                CancellationToken cancellationToken)
        {
            var query = new TopEnrollmentQuery()
            {
                Filter = filter
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }

        public static async Task<IResult> GetEnrollmentAmount(ISender sender,
                                                                CancellationToken cancellationToken)
        {
            var query = new EnrollmentAmountQuery()
            {
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }
    }
}
