using Application.Common.Helper;
using Application.Common.Messages;
using Application.Common.Models.TestContent;
using Application.Features.ZoneFeatures.Commands;
using Application.Messages;
using Application.Services.Authentication;
using AutoMapper;
using Domain.Entity;
using Domain.Models.Common;
using FluentValidation;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.AssignmentFeatures.Commands
{
    public class CreateAssignmentCommand : IRequest<APIResponse>
    {
        [JsonIgnore]
        public Guid ZoneId { get; set; }

        public string Title { get; set; }

        public string? Noticed { get; set; }

        public DateTime AvailableAt { get; set; }

        public DateTime DueAt { get; set; }

        public DateTime LockedAt { get; set; }

        public bool? Published { get; set; }
        public List<TestContentCreateModel> TestContent { get; set; }
    }
    public class CreateAssigmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
    {
        public CreateAssigmentCommandValidator()
        {
            RuleFor(p => p.Title)
                .NotNull().WithMessage("{PropertyName} is required.")
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(p => p.AvailableAt)
                .NotNull().WithMessage("{PropertyName} is required.")
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(DateTime.UtcNow).WithMessage("{PropertyName} must be after now.");

            RuleFor(p => p.DueAt)
                .NotNull().WithMessage("{PropertyName} is required.")
                .GreaterThan(p => p.AvailableAt).WithMessage("{PropertyName} must be after AvailableAt."); 

            RuleFor(p => p.LockedAt)
                .NotNull().WithMessage("{PropertyName} is required.")
                .GreaterThanOrEqualTo(p => p.DueAt).WithMessage("{PropertyName} must be equal or after DueAt.");
            RuleFor(p => p.TestContent)
                .NotNull().WithMessage("{PropertyName} is required.")
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .ForEach(testContentRule =>
                 {
                     testContentRule
                         .ChildRules(content =>
                         {
                             content.RuleFor(x => x.Answers)
                                 .Must(a => a.Count > 0 && a.Count <= 6)
                                 .WithMessage("Each question can have a maximum of 6 answers.");
                             content.RuleFor(x => x.CorrectAnswer)
                                 .Must(a => a != null && a >= 0)
                                 .WithMessage("Correct answer must be greater or equal 0.");
                             content.RuleFor(x => x.Question)
                             .Must(a => a != null && a.Length > 0)
                                 .WithMessage("Question must be not null or empty.");
                         });
                 });
        }
    }
    public class CreateAssignmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService) : IRequestHandler<CreateAssignmentCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<APIResponse> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
        {
            // Check zone exists
            var zone = await _unitOfWork.ZoneRepository.GetById(request.ZoneId);

            if (zone == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.ZoneId
                };
            }

            // Check role of user performing action
            var userId = _authenticationService.User.UserId;
            if (!await _unitOfWork.ZoneMembershipRepository.IsTeacherInZone(userId, zone.Id) && !zone.CreatedBy.Equals(userId))
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.Forbidden,
                    Message = MessageCommon.Forbidden
                };
            }
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            request.AvailableAt = TimeZoneInfo.ConvertTimeToUtc(request.AvailableAt, timeZoneInfo);
            request.DueAt = TimeZoneInfo.ConvertTimeToUtc(request.DueAt, timeZoneInfo);
            request.LockedAt = TimeZoneInfo.ConvertTimeToUtc(request.LockedAt, timeZoneInfo);
            var assignment = _mapper.Map<Assignment>(request);
            assignment.CreatedBy = _authenticationService.User.UserId;

            if (request.TestContent != null)
            {
                var testContents = request.TestContent.Select((x, index) =>
                {
                    if (x.Answers.Count() <= x.CorrectAnswer)
                    {
                        throw new ValidationException($"Correct answer is not number of answer");
                    }

                    var ketContent = _mapper.Map<TestContent>(x, opts =>
                    {
                        opts.Items["Assignmentid"] = assignment.Id;
                        opts.Items["Order"] = index + 1;
                    });

                    return ketContent;
                }).ToList();

                await _unitOfWork.TestContentRepository.CreateTestContent(testContents);
                assignment.TotalQuestion = testContents.Count();
            }

            await _unitOfWork.AssignmentRepository.Add(assignment);

            var result = await _unitOfWork.SaveChangesAsync();
            if (result)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageZone.AssignmentCreatedSuccess,
                    Data = assignment.ZoneId
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageZone.AssignmentCreatedFailed
            };

        }
    }
}
