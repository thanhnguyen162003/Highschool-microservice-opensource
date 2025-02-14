﻿using Application.Common.Ultils;
using Application.Features.DocumentFeature.Queries;
using Application.Features.EnrollmentFeature.Commands;
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
    }
}
