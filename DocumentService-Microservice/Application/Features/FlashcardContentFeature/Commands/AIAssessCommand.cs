using Application.Common.Models.FlashcardContentModel;
using Application.Common.Models;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using System.Net;
using Infrastructure.Repositories.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.AIInferface;

namespace Application.Features.FlashcardContentFeature.Commands
{
    public record AIAssessCommand : IRequest<ResponseModel>
    {
        public string UserAnswer { get; set; }
        public Guid FlashcardContentId;
    }
    public class AIAssessCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IClaimInterface claim,
        IAIService aiService)
        : IRequestHandler<AIAssessCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(AIAssessCommand request, CancellationToken cancellationToken)
        {
            var userId = claim.GetCurrentUserId;
            var fc = await unitOfWork.FlashcardContentRepository.GetFlashcardContentById(request.FlashcardContentId);
            if (fc is null)
            {
                return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy thẻ ghi nhớ");
            }

            string term = string.Empty;
            string definition = string.Empty;

            if (!string.IsNullOrEmpty(fc.FlashcardContentTerm)) term = fc.FlashcardContentTerm;
            else if (!string.IsNullOrEmpty(fc.FlashcardContentTermRichText)) term = fc.FlashcardContentTermRichText;

            if (!string.IsNullOrEmpty(fc.FlashcardContentDefinition)) definition = fc.FlashcardContentDefinition;
            else if (!string.IsNullOrEmpty(fc.FlashcardContentDefinitionRichText)) definition = fc.FlashcardContentDefinitionRichText;

            if (string.IsNullOrEmpty(term) || string.IsNullOrEmpty(definition))
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Term hoặc Definition trong Content không được để trống");
            }

            var assessResult = await aiService.AssessAnswer(term, definition, request.UserAnswer);

            return new ResponseModel(HttpStatusCode.OK, "Success", assessResult);
        }
    }
}