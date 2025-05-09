using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.RecommendedModel;
using Application.Common.Models.SubjectModel;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.RecommendedFeature.Queries;

public record TopDataQuery : IRequest<TopDataResponseModel>
{
    
}
public class TopDataQueryHandler(IUnitOfWork unitOfWork,
    ILogger<TopDataQueryHandler> logger, IMapper mapper)
    : IRequestHandler<TopDataQuery, TopDataResponseModel>
{
    public async Task<TopDataResponseModel> Handle(TopDataQuery request, CancellationToken cancellationToken)
    {
        List<SubjectResponseModel> listSubjectResponse = new List<SubjectResponseModel>();
        List<FlashcardRecommendResponseModel> listFlashcardResponse = new List<FlashcardRecommendResponseModel>();
        List<DocumentResponseModel> listDocumentResponse = new List<DocumentResponseModel>();
        var placeholderSubjects = await unitOfWork.SubjectRepository.GetPlaceHolderSubjects();
        listSubjectResponse.AddRange(mapper.Map<List<SubjectResponseModel>>(mapper.Map<List<SubjectModel>>(placeholderSubjects)));
        var flashcards = await unitOfWork.FlashcardRepository.GetFlashcardsPlaceholder();
        listFlashcardResponse.AddRange(mapper.Map<List<FlashcardRecommendResponseModel>>(flashcards));
        var documents = await unitOfWork.DocumentRepository.GetDocumentPlaceholder();
        listDocumentResponse.AddRange(mapper.Map<List<DocumentResponseModel>>(documents));
        return new TopDataResponseModel
        {
            subjects = listSubjectResponse,
            flashcards = listFlashcardResponse,
            documents = listDocumentResponse
        };
    }
}
