using Application.Common.Models.DaprModel.Theory;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DaprService.Theory
{
    public record DaprGetTheoryTips : IRequest<TheoryTipsResponseDapr>
    {
        public IEnumerable<string> TheoryIds { get; set; }
    }
    public class DaprGetTheoryTipsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetTheoryTips, TheoryTipsResponseDapr>
    {
        public async Task<TheoryTipsResponseDapr> Handle(DaprGetTheoryTips request, CancellationToken cancellationToken)
        {
            var response = new TheoryTipsResponseDapr();
            var document = await unitOfWork.TheoryRepository.GetTheoryForTips(request.TheoryIds);
            if (document == null || !document.Any())
            {
                return response; // Return an empty response if no data is found.
            }
            response = new TheoryTipsResponseDapr
            {
                TheoryId = document.Select(x => x.Id.ToString()).ToList(),
                TheoryName = document.Select(x => x.TheoryName).ToList()
            };
            return response;
        }
    }
}
