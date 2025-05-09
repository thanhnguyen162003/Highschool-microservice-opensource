using System.Net;
using Application.Common.Interfaces.CloudinaryInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Models.TheoryModel;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Bson;

namespace Application.Features.TheoryFeature.Commands;

public class TheoryFileCreateCommand : IRequest<ResponseModel>
{
    public TheoryFileUploadRequestModel TheoryFileUploadRequestModel;
    public Guid TheoryId;
}
public class TheoryFileCreateCommandHandler(
    MediaDbContext context,
    ICloudinaryService cloudinaryService,
    IProducerService producer, ILogger<TheoryFileCreateCommand> logger)
    : IRequestHandler<TheoryFileCreateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(TheoryFileCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //validate the theoryid exits or not ???
            
            if (request.TheoryFileUploadRequestModel.ImageFiles is not null)
            {
                List<TheoryFile> theoryFiles = new();
                foreach (var file in request.TheoryFileUploadRequestModel.ImageFiles)
                {
                    var result = await cloudinaryService.UploadAsync(file);
                    if (result.Error != null)
                    {
                        return new ResponseModel(HttpStatusCode.BadRequest, result.Error.Message);
                    }
                    TheoryFile theoryFile = new()
                    {
                        TheoryId = request.TheoryId,
                        UpdatedAt = DateTime.UtcNow,
                        Id = ObjectId.GenerateNewId(),
                        CreatedAt = DateTime.Now,
                        FileType = result.ResourceType,
                        File = result.Url.ToString(),
                        FileExtention = result.Format,
                        PublicId = result.PublicId
                    };
                    theoryFiles.Add(theoryFile);
                }
                await context.TheoryFiles.InsertManyAsync(theoryFiles);
            }
            if (request.TheoryFileUploadRequestModel.FileDocument is not null)
            {
                List<TheoryFile> theoryFiles = new();
                foreach (var file in request.TheoryFileUploadRequestModel.FileDocument)
                {
                    var result = await cloudinaryService.UploadAsync(file);
                    if (result.Error != null)
                    {
                        return new ResponseModel(HttpStatusCode.BadRequest, result.Error.Message);
                    }
                    TheoryFile theoryFile = new TheoryFile()
                    {
                        TheoryId = request.TheoryId,
                        UpdatedAt = DateTime.UtcNow,
                        Id = ObjectId.GenerateNewId(),
                        CreatedAt = DateTime.Now,
                        FileType = result.ResourceType,
                        File = result.Url.ToString(),
                        FileExtention = result.Format,
                        PublicId = result.PublicId
                    };
                    theoryFiles.Add(theoryFile);
                }
                await context.TheoryFiles.InsertManyAsync(theoryFiles, cancellationToken: cancellationToken);
            }
            return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.CreateSuccess);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.CreateFail);
        }
    }
}
