using Application.Common.Interfaces.AzureInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.VideoModel;
using Application.Constants;
using Application.Features.VideoFeature.Commands;
using Application.KafkaMessageModel;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Application.Endpoints;

[ApiController]
[Route("api/video")]
public class VideoController(
    ILogger<VideoController> logger,
    IBlobStorageService blobStorageService,
    IMapper mapper,
    MediaDbContext context,
    IProducerService producerService,
    ISender sender) : ControllerBase
{
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    private readonly ILogger<VideoController> _logger = logger;
    private readonly IBlobStorageService _blobStorageService = blobStorageService;
    private readonly IMapper _mapper = mapper;
    private readonly MediaDbContext _context = context;
    private readonly IProducerService _producerService = producerService;
    private readonly ISender _sender = sender;

    [HttpPost("video-chunk")]
    [Authorize("moderatorPolicy")]
    public async Task<IActionResult> UploadChunk([FromForm] IFormFile file, [FromForm] int chunkNumber, [FromForm] int totalChunks, [FromForm] string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var chunkPath = Path.Combine(_uploadPath, Path.GetFileNameWithoutExtension(fileName) + extension + $".part{chunkNumber}");

        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }

        using (var stream = new FileStream(chunkPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        _logger.LogInformation("Chunk uploaded");
        return Ok(new { Success = true });
    }

    [HttpPost("clean-cache")]
    [Authorize("moderatorPolicy")]
    public async Task<IActionResult> CleanCache([FromQuery] TimeSpan fileAgeLimit)
    {
        try
        {
            if (!Directory.Exists(_uploadPath))
            {
                return Ok(new { Success = false, Message = "No cache to clean." });
            }

            var chunkFiles = Directory.GetFiles(_uploadPath, "*.part*");
            var allFiles = Directory.GetFiles(_uploadPath, "*");
            int deletedFilesCount = 0;

            foreach (var chunkFile in chunkFiles)
            {
                var fileInfo = new FileInfo(chunkFile);
                System.IO.File.Delete(chunkFile);
                _logger.LogInformation($"Deleted old chunk file: {chunkFile}");
                deletedFilesCount++;
            }

            foreach (var allFile in allFiles)
            {
                var fileInfo = new FileInfo(allFile);
                System.IO.File.Delete(allFile);
                _logger.LogInformation($"Deleted old video file: {allFile}");
                deletedFilesCount++;
            }

            return Ok(new
            {
                Success = true,
                Message = $"{deletedFilesCount} old chunk file(s) deleted."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cleaning cache: {ex.Message}");
            return StatusCode(500, new { Success = false, Message = "Error cleaning cache." });
        }
    }

    [HttpPost("lesson/{id}/merge")]
    [Authorize("moderatorPolicy")]
    public async Task<IActionResult> MergeChunks([FromBody] MergeChunksRequest request, [FromRoute] Guid id)
    {
        var fileName = request.FileName;
        var mergedFilePath = Path.Combine(_uploadPath, fileName);

        var chunkFiles = Directory.GetFiles(_uploadPath, fileName + ".part*")
            .OrderBy(f => f);
        _logger.LogInformation($"Chunk files to merge: {string.Join(", ", chunkFiles)}");

        using (var finalFileStream = new FileStream(mergedFilePath, FileMode.Create))
        {
            foreach (var chunkFile in chunkFiles)
            {
                var chunkFileInfo = new FileInfo(chunkFile);
                _logger.LogInformation($"Chunk: {chunkFile}, Size: {chunkFileInfo.Length} bytes");
                using (var chunkStream = new FileStream(chunkFile, FileMode.Open))
                {
                    await chunkStream.CopyToAsync(finalFileStream);
                    _logger.LogInformation($"Merging chunk: {chunkFile}, size: {chunkStream.Length} bytes");
                }
            }
        }

        var fileInfo = new FileInfo(mergedFilePath);
        _logger.LogInformation($"Merged file size: {fileInfo.Length} bytes");

        var uploadResult = await UploadFileToBlobStorage(fileName, mergedFilePath); // Updated method

        if (uploadResult is not null)
        {
            VideoFile videoModel = new VideoFile()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                LessonId = id,
                VideoUrl = uploadResult,
            };
            KafkaVideoLessonUploadedModel model = new KafkaVideoLessonUploadedModel()
            {
                VideoUrl = uploadResult,
                LessonId = id
            };
            await _context.VideoFiles.InsertOneAsync(videoModel);
            var producer = await _producerService.ProduceObjectWithKeyAsync(KafkaConstaints.VideoUploaded, id.ToString(), model);
            if (!producer)
            {
                _logger.Log(LogLevel.Error, "VideoUrl ProduceFailure");
            }
            foreach (var chunkFile in chunkFiles)
            {
                System.IO.File.Delete(chunkFile);
            }
            System.IO.File.Delete(mergedFilePath);
            return Ok(new { Success = true, Message = "File merged and uploaded successfully." });
        }

        return StatusCode(500, new { Success = false, Message = "Failed to upload the merged file to Blob Storage." });
    }

    private async Task<string> UploadFileToBlobStorage(string fileName, string filePath)
    {
        try
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                string fileKey = Path.GetFileNameWithoutExtension(fileName) + "-" + Guid.NewGuid() + Path.GetExtension(fileName);
                var blobUrl = await _blobStorageService.UploadFileAsync("videos", fileKey, fileStream); 

                if (!string.IsNullOrEmpty(blobUrl))
                {
                    return blobUrl;
                }
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + " Error in UploadFileToBlobStorage");
            return null;
        }
    }

    [HttpDelete("")]
    public async Task<IActionResult> DeleteVideo([FromBody] VideoDeleteCommand videoDeleteCommand, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(videoDeleteCommand, cancellationToken);
        return StatusCode((int)result.Status!, result);
    }
}

//namespace Application.Endpoints;

//[ApiController]
//[Route("api/video")]
//public class VideoController : ControllerBase
//{
//    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
//    private readonly ILogger<VideoController> _logger;
//    private readonly IAWSS3Service _awsS3Service;
//    private readonly IMapper _mapper;
//    private readonly MediaDbContext _context;
//    private readonly LessonServiceCheckRpc.LessonServiceCheckRpcClient _client;
//    private readonly IProducerService _producerService;
//    private readonly ISender _sender;

//    public VideoController(ILogger<VideoController> logger, IAWSS3Service awsS3Service, IMapper mapper, MediaDbContext context, IProducerService producerService,
//        LessonServiceCheckRpc.LessonServiceCheckRpcClient client, ISender sender)
//    {
//        _logger = logger;
//        _awsS3Service = awsS3Service;
//        _mapper = mapper;
//        _context = context;
//        _producerService = producerService;
//        _client = client;
//        _sender = sender;
//    }

//    [HttpPost("video-chunk")]
//    [Authorize("moderatorPolicy")]
//    public async Task<IActionResult> UploadChunk([FromForm] IFormFile file, [FromForm] int chunkNumber, [FromForm] int totalChunks, [FromForm] string fileName)
//    {
//        // Ensure the file has its original extension
//        var extension = Path.GetExtension(fileName);
//        var chunkPath = Path.Combine(_uploadPath, Path.GetFileNameWithoutExtension(fileName) + extension + $".part{chunkNumber}");


//        if (!Directory.Exists(_uploadPath))
//        {
//            Directory.CreateDirectory(_uploadPath);
//        }

//        // Save the chunk to disk
//        using (var stream = new FileStream(chunkPath, FileMode.Create))
//        {
//            await file.CopyToAsync(stream);
//        }

//        _logger.LogInformation("Chunk uploaded");
//        return Ok(new { Success = true });
//    }

//    [HttpPost("clean-cache")]
//    [Authorize("moderatorPolicy")]
//    public async Task<IActionResult> CleanCache([FromQuery] TimeSpan fileAgeLimit)
//    {
//        try
//        {
//            if (!Directory.Exists(_uploadPath))
//            {
//                return Ok(new { Success = false, Message = "No cache to clean." });
//            }

//            // Get all chunk files in the directory (e.g., *.part* files)
//            var chunkFiles = Directory.GetFiles(_uploadPath, "*.part*");
//            var allFiles = Directory.GetFiles(_uploadPath, "*");
//            int deletedFilesCount = 0;
//            foreach (var chunkFile in chunkFiles)
//            {
//                var fileInfo = new FileInfo(chunkFile);
//                // Delete the file
//                System.IO.File.Delete(chunkFile);
//                _logger.LogInformation($"Deleted old chunk file: {chunkFile}");
//                deletedFilesCount++;
//            }
//            foreach (var allFile in allFiles)
//            {
//                var fileInfo = new FileInfo(allFile);
//                System.IO.File.Delete(allFile);
//                _logger.LogInformation($"Deleted old video file: {allFile}");
//                deletedFilesCount++;
//            }
//            return Ok(new
//            {
//                Success = true,
//                Message = $"{deletedFilesCount} old chunk file(s) deleted."
//            });
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError($"Error cleaning cache: {ex.Message}");
//            return StatusCode(500, new { Success = false, Message = "Error cleaning cache." });
//        }
//    }

//    [HttpPost("lesson/{id}/merge")]
//    [Authorize("moderatorPolicy")]
//    public async Task<IActionResult> MergeChunks([FromBody] MergeChunksRequest request, [FromRoute] Guid id)
//    {
//        var fileName = request.FileName;
//        var mergedFilePath = Path.Combine(_uploadPath, fileName);
//        // validate lessonId
//         // LessonCheckRequest lessonCheckRequest = new LessonCheckRequest
//         // {
//         //     LessonId = { id.ToString() }
//         // };
//         //
//         // var lessonExit = await _client.CheckLessonExitAsync(lessonCheckRequest);
//         //
//         // if (lessonExit.LessonExits.ToString().Equals("false"))
//         // {
//         //     return StatusCode(500, new { Success = false, Message = "Cant verify lesson" });
//         // }
//         //
//         var chunkFiles = Directory.GetFiles(_uploadPath, fileName + ".part*")
//             .OrderBy(f => f);
//         _logger.LogInformation($"Chunk files to merge: {string.Join(", ", chunkFiles)}");
//        using (var finalFileStream = new FileStream(mergedFilePath, FileMode.Create))
//        {
//            foreach (var chunkFile in chunkFiles)
//            {
//                var chunkFileInfo = new FileInfo(chunkFile);
//                _logger.LogInformation($"Chunk: {chunkFile}, Size: {chunkFileInfo.Length} bytes");
//                using (var chunkStream = new FileStream(chunkFile, FileMode.Open))
//                {
//                    await chunkStream.CopyToAsync(finalFileStream);
//                    _logger.LogInformation($"Merging chunk: {chunkFile}, size: {chunkStream.Length} bytes");
//                }
//            }
//        }

//        var fileInfo = new FileInfo(mergedFilePath);
//        _logger.LogInformation($"Merged file size: {fileInfo.Length} bytes");

//        var uploadResult = await UploadFileToS3(fileName, mergedFilePath);

//        if (uploadResult is not null)
//        {
//            VideoFile videoModel = new VideoFile()
//            {
//                Id = ObjectId.GenerateNewId().ToString(),
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now,
//                LessonId = id,
//                VideoUrl = uploadResult,
//            };
//            KafkaVideoLessonUploadedModel model = new KafkaVideoLessonUploadedModel()
//            {
//                VideoUrl = uploadResult,
//                LessonId = id
//            };
//            await _context.VideoFiles.InsertOneAsync(videoModel);
//            var producer = await _producerService.ProduceObjectWithKeyAsync(KafkaConstaints.VideoUploaded, id.ToString(), model);
//            if (!producer)
//            {
//                _logger.Log(LogLevel.Error, "VideoUrl ProduceFailure");
//            }
//            foreach (var chunkFile in chunkFiles)
//            {
//                System.IO.File.Delete(chunkFile);
//            }
//            System.IO.File.Delete(mergedFilePath);
//            return Ok(new { Success = true, Message = "File merged and uploaded successfully." });
//        }

//        return StatusCode(500, new { Success = false, Message = "Failed to upload the merged file to S3." });
//    }

//    private async Task<string> UploadFileToS3(string fileName, string filePath)
//    {
//        try
//        {
//            using (var fileStream = new FileStream(filePath, FileMode.Open))
//            {
//                string fileKey = Path.GetFileNameWithoutExtension(fileName) + "-" + Guid.NewGuid() + Path.GetExtension(fileName);
//                var s3Url = await _awsS3Service.UploadFileAsync("video", fileKey, fileStream);

//                if (!string.IsNullOrEmpty(s3Url))
//                {
//                    return s3Url;
//                }
//                return null;
//            }
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex.Message+"Error in UploadFileToS3");
//            return null;
//        }
//    }

//    [HttpDelete("")]
//    public async Task<IActionResult> DeleteVideo([FromBody] VideoDeleteCommand videoDeleteCommand, CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(videoDeleteCommand, cancellationToken);

//        return StatusCode((int)result.Status!, result);
//    }
//}
