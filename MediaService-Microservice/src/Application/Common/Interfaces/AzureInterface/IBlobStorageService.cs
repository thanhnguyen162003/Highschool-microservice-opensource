namespace Application.Common.Interfaces.AzureInterface;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream);
    Task<byte[]> DownloadFileAsync(string key);
    Task<bool> DeleteFileAsync(string key);
}
