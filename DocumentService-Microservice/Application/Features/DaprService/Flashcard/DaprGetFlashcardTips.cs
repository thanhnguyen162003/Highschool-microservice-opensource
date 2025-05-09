using Application.Common.Models.DaprModel.Flashcard;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DaprService.Flashcard
{
    public record DaprGetFlashcardTips : IRequest<FlashcardTipsResponseDapr>
    {
        public IEnumerable<string> FlaschcardId { get; set; }
    }
    public class DaprGetFlashcardTipsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetFlashcardTips, FlashcardTipsResponseDapr>
    {
        public async Task<FlashcardTipsResponseDapr> Handle(DaprGetFlashcardTips request, CancellationToken cancellationToken)
        {
            var flashcards = await unitOfWork.FlashcardRepository.GetFlashcardForTips(request.FlaschcardId);
            if (flashcards == null || !flashcards.Any())
            {
                return new FlashcardTipsResponseDapr(); // Return an empty response if no data is found.
            }
            var response = new FlashcardTipsResponseDapr()
            {
                FlaschcardId = flashcards.Select(f => f.Id.ToString()).ToList(),
                FlaschcardName = flashcards.Select(f => f.FlashcardName).ToList(),
                FlaschcardSlug = flashcards.Select(f => f.Slug).ToList() 
            };
            return response;
        }
    }
}
