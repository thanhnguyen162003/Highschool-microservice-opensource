using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Application.Common.Ultils
{
    public static class StringExtension
    {
        public static string ToSplitUsername(this string email)
        {
            return Regex.Match(email, @"^[^@]+").Value;
        }

        public static string GetHollandType(string hollandType)
        {
            if (string.IsNullOrEmpty(hollandType)) return string.Empty;

            hollandType = hollandType.ToUpper();

            var uniqueChars = hollandType.Distinct().ToArray();

            var allowedSet = "RIASEC";

            return uniqueChars.Length == 3 && uniqueChars.All(c => allowedSet.Contains(c)) ? hollandType : string.Empty;
        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
