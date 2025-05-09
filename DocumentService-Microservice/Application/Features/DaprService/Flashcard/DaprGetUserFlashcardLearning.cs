using Application.Common.Models.DaprModel.Flashcard;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DaprService.Flashcard
{
    public record DaprGetUserFlashcardLearning : IRequest<UserFlashcardLearningResponseDapr>
    {
    }
    public class DaprGetUserFlashcardLearningHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetUserFlashcardLearning, UserFlashcardLearningResponseDapr>
    {
        public async Task<UserFlashcardLearningResponseDapr> Handle(DaprGetUserFlashcardLearning request, CancellationToken cancellationToken)
        {
            var progress = await unitOfWork.UserFlashcardProgressRepository.GetAllProgressLearning();
            var response = new UserFlashcardLearningResponseDapr()
            {
                UserFlashcardLearning = progress.Select(p => new UserFlashcardLearningDapr
                {
                    UserId = p.UserId.ToString(),
                    FlashcardId = p.FlashcardId.ToString(),
                    FlashcardContentId = p.FlashcardContentId.ToString(),
                    LastReviewDateHistory = p.LastReviewDateHistory.Select(date => date.ToString("o")).ToList(),
                    TimeSpentHistory = p.TimeSpentHistory
                }).ToList()
            };
            return response;
        }
    }
}
