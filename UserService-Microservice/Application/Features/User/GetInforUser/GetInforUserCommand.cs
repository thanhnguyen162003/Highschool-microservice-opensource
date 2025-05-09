using Application.Common.Models.Common;

namespace Application.Features.User.GetInforUser
{
	public class GetInforUserCommand : IRequest<ResponseModel>
	{
		public string UserName { get; set; }
	}
}
