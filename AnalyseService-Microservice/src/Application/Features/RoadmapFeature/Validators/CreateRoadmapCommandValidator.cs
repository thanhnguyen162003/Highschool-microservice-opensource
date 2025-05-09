using Application.Common.Models.RoadmapDataModel;
using Application.Features.RoadmapFeature.Commands;

namespace Application.Features.RoadmapFeature.Validators;

public class CreateRoadmapCommandValidator : AbstractValidator<RoadmapCreateRequestModel>
{
    public CreateRoadmapCommandValidator()
    {
        RuleFor(v => v.RoadmapName)
            .NotEmpty().WithMessage("SectionName is required.")
            .MinimumLength(2).WithMessage("SectionName must at least 2 character")
            .MaximumLength(150).WithMessage("SectionName must not exceed 150 characters.");
        
        RuleFor(v => v.RoadmapDescription)
            .NotEmpty().WithMessage("SectionDescription is required.")
            .MinimumLength(2).WithMessage("SectionDescription must at least 2 character")
            .MaximumLength(150).WithMessage("SectionDescription must not exceed 150 characters.");

        RuleFor(v => v.RoadmapSubjectIds)
            .NotEmpty();
        
        RuleFor(v => v.TypeExam)
            .NotEmpty();
        
    }
}

