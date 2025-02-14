using Application.Common.Models.TheoryModel;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.TheoryFeature.Queries;

public record TheoryFileQuery : IRequest<List<TheoryFileResponseModel>>
{
    public Guid TheoryId;
}
public class TheoryFileQueryHandler(MediaDbContext dbContext, IMapper mapper) : IRequestHandler<TheoryFileQuery, List<TheoryFileResponseModel>>
{
    public async Task<List<TheoryFileResponseModel>> Handle(TheoryFileQuery request, CancellationToken cancellationToken)
    {
        var listTheory = await dbContext.TheoryFiles
            .Find(theoryFile => theoryFile.TheoryId.Equals(request.TheoryId)).ToListAsync(cancellationToken: cancellationToken);
        return mapper.Map<List<TheoryFileResponseModel>>(listTheory);
    }
}
