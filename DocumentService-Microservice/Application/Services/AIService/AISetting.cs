namespace Application.Services.AIService
{
	public class AISetting
	{
		public string OpenAIKey { get; set; } = null!;
		public int MaxFileSize { get; set; } // In bytes
		public int AITimeoutSeconds { get; set; }
	}
}
