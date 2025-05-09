namespace Application.Common.Models.AuthenModel;

public class RegisterRequestModel
{
	public string? Email { get; set; }
	public string? Password { get; set; }
	public string? FullName { get; set; }
}