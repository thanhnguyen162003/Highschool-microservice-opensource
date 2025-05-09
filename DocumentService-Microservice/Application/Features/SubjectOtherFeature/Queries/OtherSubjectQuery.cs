using Domain.QueriesFilter;

namespace Application.Features.SubjectOtherFeature.Queries;

public record OtherSubjectQuery
{
    public OtherSubjectQueryFilter QueryFilter { get; init; }
}