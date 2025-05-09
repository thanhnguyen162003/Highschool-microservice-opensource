using MongoDB.Driver;
using Application.Common.Models.CommonModels;
using Infrastructure.Data;
using Application.Common.Interfaces.AzureInterface;
using Domain.Entities;
using Application.Common.Models.DocumentModel;
using Application.Common.Ultils;
using Application.Constants;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Application.Common.Interfaces.CloudinaryInterface;

namespace Application.Features.DocumentFeature.Commands;

public record UploadDocumentFileCommand : IRequest<ResponseModel>
{
    public Guid DocumentId;
    public required IFormFile DocumentFile;
}

public class UploadDocumentFileCommandHandler(
    MediaDbContext dbContext,
    IBlobStorageService blobStorageService,
    ICloudinaryService cloudinary,
    ILogger<UploadDocumentFileCommandHandler> logger,
    IMapper mapper) : IRequestHandler<UploadDocumentFileCommand, ResponseModel>
{
    private readonly MediaDbContext _dbContext = dbContext;
    private readonly IBlobStorageService _blobStorageService = blobStorageService;
    private readonly ICloudinaryService _cloudinary = cloudinary;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UploadDocumentFileCommandHandler> _logger = logger;

    public async Task<ResponseModel> Handle(UploadDocumentFileCommand request, CancellationToken cancellationToken)
    {
        var updateDocument = await _dbContext.DocumentFiles
            .Find(documentFile => documentFile.DocumentId.Equals(request.DocumentId))
            .FirstOrDefaultAsync();

        DocumentFileResponseModel responseModel = new DocumentFileResponseModel();
        string originalFileName = Path.GetFileNameWithoutExtension(request.DocumentFile.FileName).Trim();
        string fileExtension = Path.GetExtension(request.DocumentFile.FileName).Trim();
        string fileKey = $"{originalFileName}-{request.DocumentId}{fileExtension}";

        bool isPdf = fileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        //string documentImagePreviewUrl = null;

        using (var fileStream = request.DocumentFile.OpenReadStream())
        {
            string urlResult = await _blobStorageService.UploadFileAsync("documents", fileKey, fileStream);

            //if (isPdf && urlResult != null)
            //{
            //    documentImagePreviewUrl = await GeneratePdfPreviewImage(request.DocumentFile, originalFileName, request.DocumentId);
            //}

            if (updateDocument != null)
            {
                if (!string.IsNullOrWhiteSpace(updateDocument.DocumentFileUrl))
                {
                    string oldKey = UrlHelper.GetBlobKeyFromUrl(updateDocument.DocumentFileUrl);
                    await _blobStorageService.DeleteFileAsync(oldKey);
                }

                var filter = Builders<DocumentFile>.Filter.Eq(d => d.DocumentId, updateDocument.DocumentId);
                var update = Builders<DocumentFile>.Update
                    .Set(d => d.DocumentFileUrl, urlResult)
                    //.Set(d => d.DocumentImagePreview, documentImagePreviewUrl)
                    ;

                await _dbContext.DocumentFiles.UpdateOneAsync(filter, update);

                updateDocument.DocumentFileUrl = urlResult;
                //updateDocument.DocumentImagePreview = documentImagePreviewUrl;
                _mapper.Map(updateDocument, responseModel);
            }
            else
            {
                DocumentFile documentFile = new DocumentFile()
                {
                    DocumentId = request.DocumentId,
                    DocumentFileUrl = urlResult,
                    DocumentFileExtension = fileExtension,
                    DocumentFileType = "Document",
                    //DocumentImagePreview = documentImagePreviewUrl
                };

                await _dbContext.DocumentFiles.InsertOneAsync(documentFile);
                _mapper.Map(documentFile, responseModel);
            }
        }

        return !string.IsNullOrWhiteSpace(responseModel.DocumentFileUrl)
            ? new ResponseModel(System.Net.HttpStatusCode.OK, ResponseConstaints.UpdateSuccessful, responseModel)
            : new ResponseModel(System.Net.HttpStatusCode.InternalServerError, ResponseConstaints.Error);
    }

    //private async Task<string> GeneratePdfPreviewImage(IFormFile pdfFile, string originalFileName, Guid documentId)
    //{
    //    try
    //    {
    //        string previewImageKey = $"{originalFileName}-preview-{documentId}";
    //        string publicId = $"document-previews/{previewImageKey}";

    //        var uploadResult = await _cloudinary.UploadRawAsync(pdfFile, publicId);
    //        _logger.LogInformation($"Uploaded PDF - PublicId: {publicId}, Status: {uploadResult.StatusCode}, URL: {uploadResult.SecureUrl}, ResourceType: {uploadResult.ResourceType}");

    //        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
    //        {
    //            _logger.LogError($"Cloudinary upload failed: {uploadResult.Error?.Message}");
    //            return null;
    //        }

    //        var transformation = new Transformation()
    //            .Width(800) 
    //            .Crop("scale");

    //        string previewImageUrl = _cloudinary.GenerateImageUrl(publicId, transformation);
    //        _logger.LogInformation($"Generated preview URL: {previewImageUrl}");

    //        return previewImageUrl;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error generating PDF preview for document {documentId}");
    //        return null;
    //    }
    //}
}
