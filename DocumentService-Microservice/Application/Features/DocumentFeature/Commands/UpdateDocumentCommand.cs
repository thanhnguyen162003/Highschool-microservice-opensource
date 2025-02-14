using Application.Common.Models;
using Application.Common.Models.DocumentModel;
using Application.Common.Ultils;
using Application.Constants;
using Application.Messages.Document;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.DocumentFeature.Commands
{
    public record UpdateDocumentCommand : IRequest<ResponseModel>
    {
        public UpdateDocumentRequestModel UpdateDocumentRequestModel;
        public Guid DocumentId;
    }
    public class UpdateDocumentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateDocumentCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            if ((request.UpdateDocumentRequestModel.SubjectId != null && request.UpdateDocumentRequestModel.CurriculumId == null)
                || (request.UpdateDocumentRequestModel.SubjectId == null && request.UpdateDocumentRequestModel.CurriculumId != null))
            {
                return new ResponseModel(System.Net.HttpStatusCode.BadRequest, "Môn học và chương trình học không được bỏ trống");
            }
            Guid subjectCurriculumId = Guid.Empty;
            if (request.UpdateDocumentRequestModel.SubjectId != null && request.UpdateDocumentRequestModel.CurriculumId != null)
            {
                var subjectExist = await unitOfWork.SubjectCurriculumRepository.Get(filter: s => s.SubjectId == request.UpdateDocumentRequestModel.SubjectId && s.CurriculumId == request.UpdateDocumentRequestModel.CurriculumId);
                if (subjectExist.IsNullOrEmpty())
                {
                    return new ResponseModel(System.Net.HttpStatusCode.BadRequest, DocumentMessage.SUBJECT_NOT_FOUND);
                }
                subjectCurriculumId = subjectExist.First().Id;
            }
            if (request.UpdateDocumentRequestModel.SchoolId != null)
            {
                var schoolExist = await unitOfWork.SchoolRepository.Get(filter: s => s.Id == request.UpdateDocumentRequestModel.SchoolId);
                if (schoolExist.IsNullOrEmpty())
                {
                    return new ResponseModel(System.Net.HttpStatusCode.BadRequest, "Không tìm thấy trường học");
                }
            }
            var finder = await unitOfWork.DocumentRepository.Get(filter: document => document.Id == request.DocumentId && document.DeletedAt == null);
            var updateDocument = finder.FirstOrDefault();
            if (updateDocument == null)
            {
                return new ResponseModel(System.Net.HttpStatusCode.NotFound, "Không tìm thấy tài liệu");
            }
            //trường hợp consume message từ media
            if (!string.IsNullOrWhiteSpace(request.UpdateDocumentRequestModel.DocumentFileName) && updateDocument.Slug!.StartsWith("default-document-name"))
            {
                updateDocument.Slug = SlugHelper.GenerateSlug(request.UpdateDocumentRequestModel.DocumentFileName.Trim(), updateDocument.Id.ToString());
                if (string.IsNullOrWhiteSpace(updateDocument.DocumentName))
                {
                    updateDocument.DocumentName = request.UpdateDocumentRequestModel.DocumentFileName;
                }
            }

            //trường hợp update bình thường
            if (!string.IsNullOrWhiteSpace(request.UpdateDocumentRequestModel.DocumentName))
            {
                updateDocument.Slug = SlugHelper.GenerateSlug(request.UpdateDocumentRequestModel.DocumentName.Trim(), updateDocument.Id.ToString());
            }

            if (request.UpdateDocumentRequestModel.IsDownloaded != null && request.UpdateDocumentRequestModel.IsDownloaded.Value == true)
            {
                updateDocument.Download++;
            }

            updateDocument.SubjectCurriculumId = subjectCurriculumId;

            mapper.Map(request.UpdateDocumentRequestModel, updateDocument);

            _ = unitOfWork.DocumentRepository.Update(updateDocument);

            int result = await unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel(System.Net.HttpStatusCode.OK, ResponseConstaints.DocumentUpdated, updateDocument.Slug);
            }
            return new ResponseModel(System.Net.HttpStatusCode.InternalServerError, ResponseConstaints.DocumentUpdateFailed);
        }
    }
}