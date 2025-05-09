using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using static Domain.Enums.ImageOption;

namespace Application.Common.Interfaces.CloudinaryInterface;

public interface ICloudinaryService
{
    Task<ImageUploadResult> UploadAsync(IFormFile file);
    Task<DeletionResult> DeleteAsync(string fileUrl);
    Task<UploadResult> UploadImage(IFormFile file, ImageFolder folder, ImageFormat format, string? fileName);
    Task<UploadResult> UploadImage(byte[] file, ImageFolder folder, ImageFormat format, string? fileName);

    // New methods for raw file upload and URL generation
    Task<RawUploadResult> UploadRawAsync(IFormFile file, string publicId);
    string GenerateImageUrl(string publicId, Transformation transformation);
}
