using Application.Common.Models;

namespace Application.Features.RoadmapFeature.Validators;

public class CreateRoadmapSectionCommandValidator : AbstractValidator<RoadMapSectionCreateRequestModel>
{
    public CreateRoadmapSectionCommandValidator()
    {
        RuleFor(v => v.Edges)
            .NotEmpty();
        
        RuleFor(v => v.ContentJson)
            .NotEmpty();
        
        RuleFor(v => v.Nodes)
            .NotEmpty();
    }
}
