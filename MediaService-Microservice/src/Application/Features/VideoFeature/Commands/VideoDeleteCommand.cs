//using System.Net;
//using Application.Common.Interfaces.AWS3ServiceInterface;
//using Application.Common.Interfaces.CloudinaryInterface;
//using Application.Common.Models.CommonModels;
//using Domain.Entities;
//using Infrastructure.Data;
//using MongoDB.Driver;

//namespace Application.Features.VideoFeature.Commands;

//public class VideoDeleteCommand : IRequest<ResponseModel>
//{
//    public string Url { get; set; } = null!;
//}

//public class VideoDeleteCommandHandler : IRequestHandler<VideoDeleteCommand, ResponseModel>
//{
//    private readonly MediaDbContext _context;
//    private readonly IAWSS3Service _awsS3Service;

//    public VideoDeleteCommandHandler(MediaDbContext context, IAWSS3Service aWSS3Service)
//    {
//        _context = context;
//        _awsS3Service = aWSS3Service;
//    }

//    public async Task<ResponseModel> Handle(VideoDeleteCommand request, CancellationToken cancellationToken)
//    {
//        var video = await _context.VideoFiles.Find(v => v.VideoUrl.Equals(request.Url)).FirstOrDefaultAsync();

//        if (video == null)
//        {
//            return new ResponseModel()
//            {
//                Status = HttpStatusCode.NotFound,
//                Message = "Video not found"
//            };
//        }

//        var result = await _context.VideoFiles.DeleteOneAsync(v => v.VideoUrl.Equals(request.Url));

//        if (result.DeletedCount > 0)
//        {
//            if(await _awsS3Service.DeleteFileAsync(video.VideoUrl))
//            {
//                return new ResponseModel()
//                {
//                    Status = HttpStatusCode.OK,
//                    Message = "Video deleted successfully"
//                };
//            }

//            return new ResponseModel()
//            {
//                Status = HttpStatusCode.InternalServerError,
//                Message = "Failed to delete video on aws"
//            };
//        }

//        return new ResponseModel()
//        {
//            Status = HttpStatusCode.InternalServerError,
//            Message = "Failed to delete video on server"
//        };
//    }
//}
using System.Net;
using Application.Common.Interfaces.AzureInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Ultils;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.VideoFeature.Commands;

public class VideoDeleteCommand : IRequest<ResponseModel>
{
    public string Url { get; set; } = null!;
}

public class VideoDeleteCommandHandler(MediaDbContext context, IBlobStorageService blobStorageService) : IRequestHandler<VideoDeleteCommand, ResponseModel>
{
    private readonly MediaDbContext _context = context;
    private readonly IBlobStorageService _blobStorageService = blobStorageService; // Updated to Azure service

    public async Task<ResponseModel> Handle(VideoDeleteCommand request, CancellationToken cancellationToken)
    {
        var video = await _context.VideoFiles.Find(v => v.VideoUrl.Equals(request.Url)).FirstOrDefaultAsync();

        if (video == null)
        {
            return new ResponseModel()
            {
                Status = HttpStatusCode.NotFound,
                Message = "Video not found"
            };
        }

        var result = await _context.VideoFiles.DeleteOneAsync(v => v.VideoUrl.Equals(request.Url));

        if (result.DeletedCount > 0)
        {
            // Extract the blob key from the Azure Blob Storage URL
            string blobKey = UrlHelper.GetBlobKeyFromUrl(video.VideoUrl);
            if (await _blobStorageService.DeleteFileAsync(blobKey))
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.OK,
                    Message = "Video deleted successfully"
                };
            }

            return new ResponseModel()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Failed to delete video on Azure Blob Storage"
            };
        }

        return new ResponseModel()
        {
            Status = HttpStatusCode.InternalServerError,
            Message = "Failed to delete video on server"
        };
    }
}

