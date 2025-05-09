using Application.Common.Extentions;
using Application.Common.Interfaces.CloudinaryInterface;
using Application.Common.Ultils;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using static Domain.Enums.ImageOption;

namespace Application.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryService> _logger;

    public CloudinaryService(IOptions<CloudinarySettings> config, ILogger<CloudinaryService> logger)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
        _logger = logger;
    }

    public async Task<UploadResult> UploadImage(IFormFile file, ImageFolder folder, ImageFormat format, string? fileName)
    {
        var name = string.IsNullOrEmpty(fileName) ? $"{DateTime.Now.Ticks}" : $"{fileName}_${DateTime.Now.Ticks}";
        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder.GetDescription(),
            Format = format.ToString(),
            PublicId = name,
            PublicIdPrefix = "/images"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        return result;
    }

    public async Task<UploadResult> UploadImage(byte[] file, ImageFolder folder, ImageFormat format, string? fileName)
    {
        var name = string.IsNullOrEmpty(fileName) ? $"{DateTime.Now.Ticks}" : $"{fileName}_${DateTime.Now.Ticks}";
        await using var stream = new MemoryStream(file);
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription("image upload", stream),
            Folder = folder.GetDescription(),
            Format = format.ToString(),
            PublicId = name,
            PublicIdPrefix = "/images"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        return result;
    }

    public async Task<ImageUploadResult> UploadAsync(IFormFile file)
    {
        if (file.Length > 0)
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream)
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result;
        }

        return new ImageUploadResult
        {
            Error = new Error { Message = "File is empty." }
        };
    }
    
    public async Task<DeletionResult> DeleteAsync(string fileUrl)
    {
        try
        {
            // Extract the public ID from the file URL
            var publicId = GetPublicIdFromUrl(fileUrl);
            if (string.IsNullOrEmpty(publicId))
            {
                throw new ArgumentException("Invalid file URL.");
            }

            // Use Cloudinary API to delete the image by its public ID
            var test = "dagqmcswa/" + publicId;
            var deleteParams = new DeletionParams(test);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image from Cloudinary.");
            return new DeletionResult
            {
                Result = "Error",
                Error = new Error
                {
                    Message = ex.Message
                }
            };
        }
    }
    
    private string GetPublicIdFromUrl(string url)
    {
        var uri = new Uri(url);
        var pathSegments = uri.AbsolutePath.Split('/');
        if (pathSegments.Length < 3) return string.Empty;
        // Assuming that the public ID comes after "v1234567890/" (version folder) in the URL
        return string.Join("/", pathSegments.Skip(2)).Split('.')[0]; // Remove the extension (e.g., ".jpg")
    }

    public async Task<RawUploadResult> UploadRawAsync(IFormFile file, string publicId)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            PublicId = publicId
        };
        return await _cloudinary.UploadAsync(uploadParams);
    }

    public string GenerateImageUrl(string publicId, Transformation transformation)
    {
        return _cloudinary.Api.Url
            .Transform(transformation)
            .Format("png")
            .BuildUrl(publicId);
    }
}
