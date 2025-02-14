using System.Text.Json;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.RecommendedModel;
using Application.Common.Models.SubjectModel;
using Domain.CustomModel;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.RecommendedFeature.Queries;

public class RecommendedDataUserQueries : IRequest<RecommendDataRecommendResponseModel>
{
}

public class RecommendedDataUserQueriesHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface, DocumentDbContext context,
    ILogger<RecommendedDataUserQueries> logger, IMapper mapper)
    : IRequestHandler<RecommendedDataUserQueries, RecommendDataRecommendResponseModel>
{
    public async Task<RecommendDataRecommendResponseModel> Handle(RecommendedDataUserQueries request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        var recommendedData = await context.RecommendedDatas.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        List<Guid>? subjectIds = recommendedData != null ? JsonSerializer.Deserialize<List<Guid>>(recommendedData.SubjectIds) : new List<Guid>();
        List<Guid>? flashcardIds = recommendedData != null ? JsonSerializer.Deserialize<List<Guid>>(recommendedData.FlashcardIds) : new List<Guid>();
        List<Guid>? documentIds = recommendedData != null ? JsonSerializer.Deserialize<List<Guid>>(recommendedData.DocumentIds) : new List<Guid>();

        List<SubjectResponseModel> listSubjectResponse = new List<SubjectResponseModel>();
        List<FlashcardRecommendResponseModel> listFlashcardResponse = new List<FlashcardRecommendResponseModel>();
        List<DocumentResponseModel> listDocumentResponse = new List<DocumentResponseModel>();

        // Process Subjects
        var subjectsToProcess = subjectIds.Count >= 5 ? subjectIds.Take(4).ToList() : subjectIds;

        foreach (var subject in subjectsToProcess)
        {
            var result = await unitOfWork.SubjectRepository.GetSubjectBySubjectId(subject, cancellationToken);
            listSubjectResponse.Add(mapper.Map<SubjectResponseModel>(result));
        }

        if (0 < subjectsToProcess.Count && subjectsToProcess.Count < 4)
        {
            int additionalSubjectsNeeded = 4 - listSubjectResponse.Count;
            var additionalSubjects = await unitOfWork.SubjectRepository.GetAdditionalSubjects(listSubjectResponse[0].CategoryName, additionalSubjectsNeeded, cancellationToken);
            listSubjectResponse.AddRange(mapper.Map<List<SubjectResponseModel>>(additionalSubjects));
        }

        if (subjectsToProcess.Count == 0)
        {
            var placeholderSubjects = await unitOfWork.SubjectRepository.GetPlaceHolderSubjects();
            listSubjectResponse.AddRange(mapper.Map<List<SubjectResponseModel>>(mapper.Map<List<SubjectModel>>(placeholderSubjects)));
        }

        // Process Flashcards
        if (flashcardIds.Count == 0 || subjectsToProcess.Count == 0)
        {
            var flashcards = await unitOfWork.FlashcardRepository.GetFlashcardsPlaceholder();
            listFlashcardResponse.AddRange(mapper.Map<List<FlashcardRecommendResponseModel>>(flashcards));
        }
        else
        {
            foreach (var flashcard in flashcardIds)
            {
                var result = await unitOfWork.FlashcardRepository.GetFlashcardById(flashcard);
                listFlashcardResponse.Add(mapper.Map<FlashcardRecommendResponseModel>(result));
            }
        }

        // Process Documents
        if (documentIds.Count == 0 || subjectsToProcess.Count == 0)
        {
            var documents = await unitOfWork.DocumentRepository.GetDocumentPlaceholder();
            listDocumentResponse.AddRange(mapper.Map<List<DocumentResponseModel>>(documents));
        }
        else
        {
            foreach (var document in documentIds)
            {
                var result = await unitOfWork.DocumentRepository.GetDocumentsByIds(document, cancellationToken);
                listDocumentResponse.Add(mapper.Map<DocumentResponseModel>(result));
            }
        }

        return new RecommendDataRecommendResponseModel
        {
            subjects = listSubjectResponse,
            flashcards = listFlashcardResponse,
            documents = listDocumentResponse,
            userId = userId
        };
    }
}
