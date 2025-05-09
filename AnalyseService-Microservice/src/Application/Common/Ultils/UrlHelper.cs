namespace Application.Common.Ultils;

public static class UrlHelper
{
    public static string GetS3KeyFromUrl(string url)
    {
        var uri = new Uri(url);
        string key = uri.AbsolutePath.TrimStart('/');
        return key;
    }
}
