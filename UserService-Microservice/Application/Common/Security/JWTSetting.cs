namespace Domain.Common.Security;

public class JWTSetting
{
	public string? SecurityKey { get; set; }
	public string? Issuer { get; set; }
	public string? Audience { get; set; }
    public int? TokenExpiry { get; set; }
    public int? RefreshTokenExpiry { get; set; }
}