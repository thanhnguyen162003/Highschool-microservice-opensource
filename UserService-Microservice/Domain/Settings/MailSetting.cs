namespace Domain.Settings
{
	public class MailSetting
	{
		public string Mail { get; set; } = null!;
		public string SmtpServer { get; set; } = null!;
		public int Port { get; set; }
		public string DisplayName { get; set; } = null!;
		public string Password { get; set; } = null!;
	}
}
