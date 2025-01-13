namespace Application.Common.Interfaces.AWS3ServiceInterface;

public interface IAWSS3Service
{
    Task<string> UploadFileAsync(string folderName, string fileName, Stream fileStream);
    Task<byte[]> DownloadFileAsync(string key);
    Task<bool> DeleteFileAsync(string key);
}
