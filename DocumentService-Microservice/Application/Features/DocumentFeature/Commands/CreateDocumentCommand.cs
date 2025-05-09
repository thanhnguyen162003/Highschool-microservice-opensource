using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.SearchModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.Messages.Document;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.DocumentFeature.Commands
{
    public record CreateDocumentCommand : IRequest<ResponseModel>
    {
        public required CreateDocumentRequestModel CreateDocumentRequestModel;
    }
    public class CreateDocumentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claim,
        IProducerService producer) : IRequestHandler<CreateDocumentCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {
            if ((request.CreateDocumentRequestModel.SubjectId != null && request.CreateDocumentRequestModel.CurriculumId == null)
                || (request.CreateDocumentRequestModel.SubjectId == null && request.CreateDocumentRequestModel.CurriculumId != null))
            {
                return new ResponseModel(System.Net.HttpStatusCode.BadRequest, "Môn học và chương trình học không được bỏ trống");
            }
            Guid subjectCurriculumId = Guid.Empty;
            if (request.CreateDocumentRequestModel.SubjectId != null && request.CreateDocumentRequestModel.CurriculumId != null)
            {
                var subjectExist = await unitOfWork.SubjectCurriculumRepository.Get(filter: s => s.SubjectId == request.CreateDocumentRequestModel.SubjectId && s.CurriculumId == request.CreateDocumentRequestModel.CurriculumId);
                if (subjectExist.IsNullOrEmpty())
                {
                    return new ResponseModel(System.Net.HttpStatusCode.BadRequest, DocumentMessage.SUBJECT_NOT_FOUND);
                }
                subjectCurriculumId = subjectExist.First().Id;
            }
            if (request.CreateDocumentRequestModel.SchoolId != null)
            {
                var schoolExist = await unitOfWork.SchoolRepository.Get(filter: s => s.Id == request.CreateDocumentRequestModel.SchoolId);
                if (schoolExist.IsNullOrEmpty())
                {
                    return new ResponseModel(System.Net.HttpStatusCode.BadRequest, "Không tìm thấy trường học");
                }
            }
                
            var newDocument = mapper.Map<Document>(request.CreateDocumentRequestModel);
            newDocument.Id = new UuidV7().Value;
            newDocument.Slug = SlugHelper.GenerateSlug(newDocument.DocumentName ?? "default document name", newDocument.Id.ToString());
            newDocument.CreatedBy = claim.GetCurrentUserId;
            newDocument.UpdatedBy = claim.GetCurrentUserId;
            newDocument.SubjectCurriculumId = subjectCurriculumId;

            _ = await unitOfWork.DocumentRepository.InsertAsync(newDocument);
            int result = 0;
            try
            {
                result = await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
            
            if (result > 0)
            {
                return new ResponseModel(System.Net.HttpStatusCode.Created, ResponseConstaints.DocumentCreated, newDocument.Id.ToString());
            }
            return new ResponseModel(System.Net.HttpStatusCode.InternalServerError, ResponseConstaints.DocumentCreateFailed);

        }
    }
}
