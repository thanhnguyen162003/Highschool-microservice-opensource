using System.Net;

namespace Application.Common.Models.External
{
	public class ImageResponse
	{
		public HttpStatusCode Status { get; set; }
		public string Message { get; set; } = string.Empty;
		public string? Url { get; set; } = string.Empty;
		public string NameFile { get; set; } = string.Empty;
	}
}
