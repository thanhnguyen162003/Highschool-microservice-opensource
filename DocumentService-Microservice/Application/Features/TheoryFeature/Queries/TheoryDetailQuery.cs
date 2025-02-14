using Application.Common.Models.TheoryModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.TheoryFeature.Queries;

public class TheoryDetailQuery : IRequest<TheoryResponseModel>
{
    public Guid TheoryId;
}

public class TheoryDetailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<TheoryDetailQuery, TheoryResponseModel>
{
    public async Task<TheoryResponseModel> Handle(TheoryDetailQuery request, CancellationToken cancellationToken)
    {
        var theory = await unitOfWork.TheoryRepository.GetTheoryByIdAsync(request.TheoryId, cancellationToken);
        return mapper.Map<TheoryResponseModel>(theory);
    }
}