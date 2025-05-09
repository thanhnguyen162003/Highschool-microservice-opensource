namespace Application.Common.Ultils;

public static class UrlHelper
{
    public static string GetS3KeyFromUrl(string url)
    {
        var uri = new Uri(url);
        string key = uri.AbsolutePath.TrimStart('/');
        return key;
    }
    public static string GetBlobKeyFromUrl(string url)
    {
        var uri = new Uri(url);
        return uri.AbsolutePath.TrimStart('/'); // Extract the path (e.g., "documents/filename.pdf")
    }
}
