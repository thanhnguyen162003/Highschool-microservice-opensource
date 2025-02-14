using CloudinaryDotNet.Actions;

namespace Application.Common.Interfaces;

public interface ICloudinaryService
{
    Task<ImageUploadResult> UploadAsync(IFormFile file);
    Task<DeletionResult> DeleteAsync(string fileUrl);
}