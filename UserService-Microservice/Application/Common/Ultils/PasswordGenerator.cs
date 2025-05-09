using System.Text;

namespace Application.Common.Ultils
{
	public static class PasswordGenerator
	{
		private static readonly Random random = new Random();
		private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
		private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string NumericChars = "0123456789";
		private const string SpecialChars = "!@#$%^&*()-_=+";

		public static string GenerateRandomPassword()
		{
			StringBuilder password = new StringBuilder();

			// Add at least 1 lowercase letter
			password.Append(LowercaseChars[random.Next(LowercaseChars.Length)]);

			// Add at least 1 uppercase letter
			password.Append(UppercaseChars[random.Next(UppercaseChars.Length)]);

			// Add at least 1 number
			password.Append(NumericChars[random.Next(NumericChars.Length)]);

			// Add at least 1 special character
			password.Append(SpecialChars[random.Next(SpecialChars.Length)]);

			// Add remaining characters randomly
			for (int i = 4; i < 9; i++)
			{
				string allChars = LowercaseChars + UppercaseChars + NumericChars + SpecialChars;
				password.Append(allChars[random.Next(allChars.Length)]);
			}

			// Shuffle the password characters
			string shuffledPassword = new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());

			return shuffledPassword;
		}

		public static string GenerateRandomPassword(int length)
		{
			StringBuilder password = new StringBuilder();

			// Add at least 1 lowercase letter
			password.Append(LowercaseChars[random.Next(LowercaseChars.Length)]);

			// Add at least 1 uppercase letter
			password.Append(UppercaseChars[random.Next(UppercaseChars.Length)]);

			// Add at least 1 number
			password.Append(NumericChars[random.Next(NumericChars.Length)]);

			// Add at least 1 special character
			password.Append(SpecialChars[random.Next(SpecialChars.Length)]);

			// Add remaining characters randomly
			for (int i = 4; i < length; i++)
			{
				string allChars = LowercaseChars + UppercaseChars + NumericChars + SpecialChars;
				password.Append(allChars[random.Next(allChars.Length)]);
			}

			// Shuffle the password characters
			string shuffledPassword = new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());

			return shuffledPassword;
		}
	}
}
