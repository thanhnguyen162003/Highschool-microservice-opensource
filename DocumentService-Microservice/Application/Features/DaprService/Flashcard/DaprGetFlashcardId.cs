using Application.Common.Models.DaprModel.Flashcard;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DaprService.Flashcard
{
    public record DaprGetFlashcardId : IRequest<FlashcardResponseDapr>
    {
        public IEnumerable<string> SubjectIds { get; set; }
    }
    public class DaprGetFlashcardIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetFlashcardId, FlashcardResponseDapr>
    {
        public async Task<FlashcardResponseDapr> Handle(DaprGetFlashcardId request, CancellationToken cancellationToken)
        {
            var document = await unitOfWork.FlashcardRepository.GetFlashcardBySubjectId(request.SubjectIds);

            var response = new FlashcardResponseDapr();
            response.FlashcardId.AddRange(document);
            return response;
        }
    }
}
