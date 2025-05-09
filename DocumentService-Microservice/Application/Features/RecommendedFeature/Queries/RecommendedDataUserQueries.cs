using System.Text.Json;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.RecommendedModel;
using Application.Common.Models.SubjectModel;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.RecommendedFeature.Queries;

public class RecommendedDataUserQueries : IRequest<RecommendDataRecommendResponseModel>
{
}

public class RecommendedDataUserQueriesHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claimInterface,
    DocumentDbContext context,
    ILogger<RecommendedDataUserQueries> logger,
    IMapper mapper)
    : IRequestHandler<RecommendedDataUserQueries, RecommendDataRecommendResponseModel>
    {
    public async Task<RecommendDataRecommendResponseModel> Handle(RecommendedDataUserQueries request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        var recommendedData = await context.RecommendedDatas
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        List<Guid>? masterSubjectIds = recommendedData != null
            ? JsonSerializer.Deserialize<List<Guid>>(recommendedData.SubjectIds)
            : new List<Guid>();
        int? grade = recommendedData?.Grade;

        List<SubjectResponseModel> listSubjectResponse = new List<SubjectResponseModel>();
        List<FlashcardRecommendResponseModel> listFlashcardResponse = new List<FlashcardRecommendResponseModel>();
        List<DocumentResponseModel> listDocumentResponse = new List<DocumentResponseModel>();

        HashSet<Guid> uniqueSubjectIds = new HashSet<Guid>();
        HashSet<Guid> uniqueFlashcardIds = new HashSet<Guid>();
        HashSet<Guid> uniqueDocumentIds = new HashSet<Guid>();

        var masterSubjectsToProcess = masterSubjectIds.Count >= 4 ? masterSubjectIds.Take(4).ToList() : masterSubjectIds;

        foreach (var masterSubjectId in masterSubjectsToProcess)
        {
            var subjects = await unitOfWork.SubjectRepository.GetSubjectsByMasterSubjectId(masterSubjectId, grade, cancellationToken);
            foreach (var subject in subjects)
            {
                if (uniqueSubjectIds.Add(subject.Id) && listSubjectResponse.Count < 4)
                {
                    listSubjectResponse.Add(mapper.Map<SubjectResponseModel>(subject));
                }
            }
        }

        if (listSubjectResponse.Count < 4)
        {
            int additionalSubjectsNeeded = 4 - listSubjectResponse.Count;
            var additionalSubjects = await unitOfWork.SubjectRepository.GetAdditionalSubjectsByCategory(
                additionalSubjectsNeeded,
                grade,
                cancellationToken);
            foreach (var subject in additionalSubjects)
            {
                if (uniqueSubjectIds.Add(subject.Id) && listSubjectResponse.Count < 4)
                {
                    listSubjectResponse.Add(mapper.Map<SubjectResponseModel>(subject));
                }
            }
        }

        if (listSubjectResponse.Count < 4)
        {
            int remainingSubjectsNeeded = 4 - listSubjectResponse.Count;
            var placeholderSubjects = await unitOfWork.SubjectRepository.GetPlaceHolderSubjects(grade);
            foreach (var subject in placeholderSubjects)
            {
                if (uniqueSubjectIds.Add(subject.Id) && listSubjectResponse.Count < 4)
                {
                    listSubjectResponse.Add(mapper.Map<SubjectResponseModel>(subject));
                }
            }
        }

        if (masterSubjectsToProcess.Any())
        {
            foreach (var masterSubjectId in masterSubjectsToProcess)
            {
                var flashcards = await unitOfWork.FlashcardRepository.GetFlashcardsByMasterSubjectId(masterSubjectId, grade, cancellationToken);
                foreach (var flashcard in flashcards)
                {
                    if (uniqueFlashcardIds.Add(flashcard.Id) && listFlashcardResponse.Count < 8)
                    {
                        listFlashcardResponse.Add(mapper.Map<FlashcardRecommendResponseModel>(flashcard));
                    }
                }
            }
        }

        if (listFlashcardResponse.Count < 5)
        {
            int additionalFlashcardsNeeded = 8 - listFlashcardResponse.Count;
            var additionalFlashcards = await unitOfWork.FlashcardRepository.GetFlashcardsPlaceholder(grade);
            if(additionalFlashcards.Count() < 8)
            {
                var missingFc = await unitOfWork.FlashcardRepository.GetFlashcardsPlaceholderNoGrade(8 - additionalFlashcards.Count());
                additionalFlashcards.AddRange(missingFc);
            }
            foreach (var flashcard in additionalFlashcards)
            {
                if (uniqueFlashcardIds.Add(flashcard.Id) && listFlashcardResponse.Count < 8)
                {
                    listFlashcardResponse.Add(mapper.Map<FlashcardRecommendResponseModel>(flashcard));
                }
            }
        }

        if (listFlashcardResponse.Count > 8)
        {
            listFlashcardResponse = listFlashcardResponse.Take(8).ToList();
        }

        if (masterSubjectsToProcess.Any())
        {
            foreach (var masterSubjectId in masterSubjectsToProcess)
            {
                var documents = await unitOfWork.DocumentRepository.GetDocumentsByMasterSubjectId(masterSubjectId, grade, cancellationToken);
                foreach (var document in documents)
                {
                    if (uniqueDocumentIds.Add(document.Id) && listDocumentResponse.Count < 8)
                    {
                        listDocumentResponse.Add(mapper.Map<DocumentResponseModel>(document));
                    }
                }
            }
        }

        if (listDocumentResponse.Count < 5)
        {
            int additionalDocumentsNeeded = 8 - listDocumentResponse.Count; 
            var additionalDocuments = await unitOfWork.DocumentRepository.GetDocumentPlaceholder(grade);
            if (additionalDocuments.Count() < 8)
            {
                var missingFc = await unitOfWork.DocumentRepository.GetDocumentPlaceholderNoGrade(8 - additionalDocuments.Count());
                additionalDocuments.AddRange(missingFc);
            }
            foreach (var document in additionalDocuments)
            {
                if (uniqueDocumentIds.Add(document.Id) && listDocumentResponse.Count < 8)
                {
                    listDocumentResponse.Add(mapper.Map<DocumentResponseModel>(document));
                }
            }
        }

        // Trim documents to 8 if exceeded
        if (listDocumentResponse.Count > 8)
        {
            listDocumentResponse = listDocumentResponse.Take(8).ToList();
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