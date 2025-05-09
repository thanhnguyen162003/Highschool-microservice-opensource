using Application.Common.Models.Common;

namespace Application.Features.User.CreateAccount
{
	public class CreateAccountCommand : IRequest<ResponseModel>
	{
		public string? Username { get; set; }

		public string? Email { get; set; } = null!;

		public string? Bio { get; set; }

		public string? Fullname { get; set; }

		public string? Password { get; set; }

		public string? ProfilePicture { get; set; }
	}
}
