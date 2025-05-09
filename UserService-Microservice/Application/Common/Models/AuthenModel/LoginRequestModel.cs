namespace Application.Common.Models.AuthenModel;

public class LoginRequestModel
{
	public string Username { get; set; } = null!;
	public string Password { get; set; } = null!;
}