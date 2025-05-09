namespace Application.Common.Models.AuthenModel;

public class LoginResponseModel
{
	public Guid UserId { get; set; }

	public string Fullname { get; set; } = null!;

	public string Email { get; set; } = null!;

	public string Username { get; set; } = null!;

	public string Image { get; set; } = null!;

	public string ProgressStage { get; set; } = string.Empty;

    public string? RoleName { get; set; }

    public DateTime? LastLoginAt { get; set; }

	public Guid SessionId { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
}