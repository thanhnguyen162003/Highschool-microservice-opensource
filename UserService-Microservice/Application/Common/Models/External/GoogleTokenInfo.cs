using Newtonsoft.Json;

namespace Application.Common.Models.External
{
	public class GoogleTokenInfo
	{
		[JsonProperty("azp")]
		public string? AuthorizedParty { get; set; }

		[JsonProperty("aud")]
		public string? Audience { get; set; }

		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonProperty("email")]
		public string? Email { get; set; }
	}
}
