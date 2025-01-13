using Application.Common.Interfaces.AWS3ServiceInterface;
using Application.Common.Models;
using Application.Common.Models.DocumentModel;
using Application.Common.Ultils;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.DocumentFeature.Commands;

public record UploadDocumentFileCommand : IRequest<ResponseModel>
{
    public Guid DocumentId;
    public IFormFile DocumentFile;
}
public class UploadDocumentFileCommandHandler(MediaDbContext dbContext, IAWSS3Service aWSS3Service, IMapper mapper) : IRequestHandler<UploadDocumentFileCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UploadDocumentFileCommand request, CancellationToken cancellationToken)
    {
        //TODO_THANH: add file size, file extension VALIDATION
        //TODO_THANH: combine png to pdf
        //TODO_THANH: ERROR_HANDLING
        //TODO_THANH: consume delete event
        //TODO_THANH: produce upload event
        var updateDocument = await dbContext.DocumentFiles
                            .Find(documentFile => documentFile.DocumentId.Equals(request.DocumentId))
                            .FirstOrDefaultAsync();
        DocumentFileResponseModel responseModel = new DocumentFileResponseModel();
        string originalFileName = Path.GetFileNameWithoutExtension(request.DocumentFile.FileName).Trim();
        string fileExtension = Path.GetExtension(request.DocumentFile.FileName).Trim();

        string fileKey = $"{originalFileName}-{request.DocumentId}{fileExtension}";
        if (updateDocument != null)
        {
            var fileStream = request.DocumentFile.OpenReadStream();
            string urlResult = "";
            urlResult = await aWSS3Service.UploadFileAsync("document", fileKey, fileStream);
            if (urlResult != null)
            {
                await aWSS3Service.DeleteFileAsync(UrlHelper.GetS3KeyFromUrl(updateDocument.DocumentFileUrl));

                updateDocument.DocumentFileUrl = urlResult;

                var filter = Builders<DocumentFile>.Filter.Eq(d => d.DocumentId, updateDocument.DocumentId);
                var update = Builders<DocumentFile>.Update.Set(d => d.DocumentFileUrl, updateDocument.DocumentFileUrl);
                await dbContext.DocumentFiles.UpdateOneAsync(filter, update);
                mapper.Map(updateDocument, responseModel);
            }
        }
        else
        {
            var fileStream = request.DocumentFile.OpenReadStream();
            string urlResult = "";
            urlResult = await aWSS3Service.UploadFileAsync("document", fileKey, fileStream);
            if (urlResult != null)
            {
                DocumentFile documentFile = new DocumentFile()
                {
                    DocumentId = request.DocumentId,
                    DocumentFileUrl = urlResult,
                    DocumentFileExtension = Path.GetExtension(request.DocumentFile.FileName),
                    DocumentFileType = "Document",
                };
                await dbContext.DocumentFiles.InsertOneAsync(documentFile);
                mapper.Map(documentFile, responseModel);
            }
        }
        if (!string.IsNullOrWhiteSpace(responseModel.DocumentFileUrl))
        {
            return new ResponseModel(System.Net.HttpStatusCode.OK, ResponseConstaints.UpdateSuccessful, responseModel);
        }
        else
        {
            return new ResponseModel(System.Net.HttpStatusCode.InternalServerError, ResponseConstaints.Error);
        }


    }
}
