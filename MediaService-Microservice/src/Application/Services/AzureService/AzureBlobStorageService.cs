using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Application.Common.Interfaces.AzureInterface;

namespace Application.Services.AzureService;

public class AzureBlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration) : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient;
    private readonly string _containerName = configuration["Azure:ContainerName"];

    public async Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobClient = containerClient.GetBlobClient(fileName);

            // Determine file type and set headers
            var extension = Path.GetExtension(fileName).ToLower();
            var blobHttpHeaders = new BlobHttpHeaders();

            switch (extension)
            {
                case ".pdf":
                    blobHttpHeaders.ContentType = "application/pdf";
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    blobHttpHeaders.ContentType = $"image/{extension.TrimStart('.')}";
                    break;
                case ".mp4":
                case ".mov":
                case ".avi":
                    blobHttpHeaders.ContentType = $"video/{extension.TrimStart('.')}";
                    break;
                default:
                    blobHttpHeaders.ContentType = "application/octet-stream"; // Generic binary
                    break;
            }

            // Upload the file (overwrites by default if the blob exists)
            await blobClient.UploadAsync(fileStream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            });

            return blobClient.Uri.ToString();
        }
        catch (Azure.RequestFailedException ex)
        {
            Console.WriteLine($"Error uploading to Azure Blob Storage: '{ex.Message}'");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unknown error: '{ex.Message}'");
            return null;
        }
    }

    public async Task<byte[]> DownloadFileAsync(string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(key);

        using var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public async Task<bool> DeleteFileAsync(string key)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(key);

            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }
        catch (Azure.RequestFailedException ex)
        {
            Console.WriteLine($"Error deleting from Azure Blob Storage: '{ex.Message}'");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unknown error: '{ex.Message}'");
            return false;
        }
    }
}
