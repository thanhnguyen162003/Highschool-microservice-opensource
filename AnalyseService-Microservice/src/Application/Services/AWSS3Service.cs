using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces.AWS3ServiceInterface;

namespace Application.Services;
public class AWSS3Service(IAmazonS3 s3Client, IConfiguration configuration) : IAWSS3Service
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly string _bucketName = configuration["AWS:BucketName"];
    private readonly string _awsRegion = configuration["AWS:Region"];

    // Upload file to S3
    public async Task<string> UploadFileAsync(string folderName, string fileName, Stream fileStream)
    {
        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{folderName}/{fileName}",  // Ensure no leading slash
                InputStream = fileStream,
                AutoCloseStream = true  // Automatically close stream after upload
            };

            var response = await _s3Client.PutObjectAsync(putRequest);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                // Construct the file URL
                string fileUrl = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{folderName}/{fileName}";
                return fileUrl;  // Return the URL on successful upload
            }

            return null;  // Return null if upload failed
        }
        catch (AmazonS3Exception ex)
        {
            // Log the error (use a proper logging mechanism in production)
            Console.WriteLine($"Error encountered on server: '{ex.Message}'");
            return null;
        }
        catch (Exception ex)
        {
            // Log other exceptions
            Console.WriteLine($"Unknown error encountered: '{ex.Message}'");
            return null;
        }
    }


    // Download file from S3
    public async Task<byte[]> DownloadFileAsync(string key)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        using var response = await _s3Client.GetObjectAsync(getRequest);
        using var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream);
        return memoryStream.ToArray(); // Return file as byte array
    }
    public async Task<bool> DeleteFileAsync(string key)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
            };

            var response = await _s3Client.DeleteObjectAsync(deleteRequest);

            // Check if the deletion was successful
            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }
        catch (AmazonS3Exception ex)
        {
            // Log the error (use a proper logging mechanism in production)
            Console.WriteLine($"Error encountered on server: '{ex.Message}'");
            return false;
        }
        catch (Exception ex)
        {
            // Log other exceptions
            Console.WriteLine($"Unknown error encountered: '{ex.Message}'");
            return false;
        }
    }
}
