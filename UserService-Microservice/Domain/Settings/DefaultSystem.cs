namespace Domain.Settings
{
	public class DefaultSystem
	{
		/// <summary>
		/// Size File is 1GB
		/// </summary>
		public int LimitSizeFile { get; set; } = 1073741824;

		/// <summary>
		/// Time to cache is 60 minutes
		/// </summary>
		public int CacheTime { get; set; } = 60;

		/// <summary>
		/// Time to verify account is 5 minutes
		/// </summary>
		public int TimeVerify { get; set; } = 10;

		/// <summary>
		/// Time to reset OTP is 5 minutes
		/// </summary>
		public int TimeToResetOTP { get; set; } = 5;

		/// <summary>
		/// Name project
		/// </summary>
		public string Abbreviation { get; set; } = string.Empty;
	}
}
