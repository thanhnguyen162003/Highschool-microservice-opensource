using Application.Common.Models;
using Application.Common.Models.BaseModelRoadmap;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.BaseFeature.Queries;

public record DataQuery : IRequest<NodeResponseModel>
{
    public Guid Id { get; init; }
}

public class DataQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<DataQuery, NodeResponseModel>
{
    public async Task<NodeResponseModel> Handle(DataQuery request, CancellationToken cancellationToken)
    {
        NodeResponseModel nodeResponseModel = new NodeResponseModel();
        var subject = await unitOfWork.SubjectRepository.GetSubjectBySubjectId(request.Id, cancellationToken);
        if (subject is not null)
        {
            PopulateNodeResponseModel(nodeResponseModel, subject, "subject");

            if (subject.Category != null)
            {
                var relatedClassData = await unitOfWork.SubjectRepository.GetSubjectsRelatedClass(subject.Category, cancellationToken);
                nodeResponseModel.RelationDocument = relatedClassData.Select(r => new RelatedDocumentResponse
                {
                    Id = r.Id,
                    Name = r.SubjectName,
                    Slug = r.Slug
                }).ToList();
            }

            return nodeResponseModel;
        }
        var subjectCurriculum = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculumById(request.Id, cancellationToken);
        if (subjectCurriculum is not null)
        {
            PopulateNodeResponseModel(nodeResponseModel, subjectCurriculum, "subject_curriculum");
            var relatedSubjectCurriculumData = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculaRelated(subjectCurriculum.CurriculumId, cancellationToken);
            if (relatedSubjectCurriculumData is not null)
            {
                nodeResponseModel.RelationDocument = relatedSubjectCurriculumData.Select(r => new RelatedDocumentResponse
                {
                    Id = r.Id,
                    Name = r.SubjectCurriculumName,
                    Slug = null
                }).ToList();
            }
            return nodeResponseModel;
        }

        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardById(request.Id);
        if (flashcard is not null)
        {
            PopulateNodeResponseModel(nodeResponseModel, flashcard, "flashcard");

            var relatedFlashcards = await unitOfWork.FlashcardRepository.GetFlashcardsBySubjectId(flashcard.SubjectId, cancellationToken);
            nodeResponseModel.RelationDocument = relatedFlashcards.Select(f => new RelatedDocumentResponse
            {
                Id = f.Id,
                Name = f.FlashcardName,
                Slug = f.Slug
            }).ToList();

            return nodeResponseModel;
        }

        // Handling Document
        var documentList = await unitOfWork.DocumentRepository.Get(d => d.Id == request.Id, includeProperties: d => d.SubjectCurriculum);
        var document = documentList.FirstOrDefault();
        if (document is not null && document.SubjectCurriculumId.HasValue)
        {
            PopulateNodeResponseModel(nodeResponseModel, document, "document");

            var relatedDocuments = await unitOfWork.DocumentRepository.GetRelatedDocuments(document.SubjectCurriculum.SubjectId, cancellationToken);
            
            nodeResponseModel.RelationDocument = relatedDocuments.Select(d => new RelatedDocumentResponse
            {
                Id = d.Id,
                Name = d.DocumentName,
                Slug = d.Slug
            }).ToList();

            return nodeResponseModel;
        }
        return nodeResponseModel;
    }

    private void PopulateNodeResponseModel<T>(NodeResponseModel model, T entity, string typeDocument)
    {
        model.Id = (Guid)entity.GetType().GetProperty("Id").GetValue(entity);
        model.Title = (string)entity.GetType().GetProperty($"{typeDocument.Capitalize()}Name").GetValue(entity);
        model.TypeDocument = typeDocument;
        model.Description = (string)entity.GetType().GetProperty($"{typeDocument.Capitalize()}Description").GetValue(entity);
        model.Slug = (string)entity.GetType().GetProperty("Slug").GetValue(entity);
    }
}

/// <summary>
/// Extension method to capitalize the first letter of a string
/// </summary>
public static class StringExtensions
{
    public static string Capitalize(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input[0].ToString().ToUpper() + input.Substring(1)
        };
}