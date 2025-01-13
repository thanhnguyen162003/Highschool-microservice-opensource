using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Common.Ultils;

public static class SlugHelper
{
    public static string GenerateSlug(string name, string? id = null)
    {
        // Convert the name to lowercase and remove accents
        string str = name.ToLowerInvariant();
        str = RemoveAccents(str);

        // Remove invalid characters
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

        // Replace multiple spaces with a single space
        str = Regex.Replace(str, @"\s+", " ").Trim();

        // Replace spaces with hyphens
        str = Regex.Replace(str, @"\s", "-");

        // Combine the processed name with the id
        return (id == null) ? $"{str}" : $"{str}-{id}";
    }

    private static string RemoveAccents(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
