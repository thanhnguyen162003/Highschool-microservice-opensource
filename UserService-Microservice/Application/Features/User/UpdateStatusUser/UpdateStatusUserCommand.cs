using Application.Common.Models.Common;
using Domain.Enumerations;

namespace Application.Features.User.UpdateStatusUser
{
	public class UpdateStatusUserCommand : IRequest<ResponseModel>
	{
		public Guid UserId { get; set; }
		public string Status { get; set; } = null!;
	}
}
