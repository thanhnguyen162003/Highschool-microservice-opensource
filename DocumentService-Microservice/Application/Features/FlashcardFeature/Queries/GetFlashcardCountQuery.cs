using Application.Common.Models.FlashcardModel;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Queries
{
    public class GetFlashcardCountQuery : IRequest<FlashcardCountResponseModel>
    {
        public string Type { get; set; }
    }
    public class GetFlashcardCountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetFlashcardCountQuery, FlashcardCountResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<FlashcardCountResponseModel> Handle(GetFlashcardCountQuery request, CancellationToken cancellationToken)
        {
            var total = await _unitOfWork.FlashcardRepository.GetTotalFlashcard();
            var thisMonth = await _unitOfWork.FlashcardRepository.GetTotalThisMonthFlashcard();
            var lastMonth = await _unitOfWork.FlashcardRepository.GetTotalLastMonthFlashcard();
            double percent = 0;
            if (lastMonth == 0)
            {
                percent = (thisMonth - lastMonth) * 100 / 1;
            }
            else
            {
                percent = (thisMonth - lastMonth) * 100 / lastMonth;
            }
            var draft = await _unitOfWork.FlashcardRepository.GetTotalFlashcardDraft();
            var open = await _unitOfWork.FlashcardRepository.GetTotalFlashcardOpen();
            var link = await _unitOfWork.FlashcardRepository.GetTotalFlashcardLink();
            return new FlashcardCountResponseModel()
            {
                TotalFlashcard = total,
                ThisMonthFlashcard = thisMonth,
                IncreaseFlashcardPercent = percent,
                TotalFlashcardDraft = draft,
                TotalFlashcardOpen = open,
                TotalFlashcardLink = link
            };            
        }
    }
}
