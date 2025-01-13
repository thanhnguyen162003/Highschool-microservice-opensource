using CloudinaryDotNet.Actions;

namespace Application.Common.Interfaces.CloudinaryInterface;

public interface ICloudinaryService
{
    Task<ImageUploadResult> UploadAsync(IFormFile file);
    Task<DeletionResult> DeleteAsync(string fileUrl);

}
