using Application.Common.Models.UserModel;
using Domain.Common;
using Domain.Common.Models;

namespace Application.Features.User.GetAllUser
{
	public class GetAllUserQuery : IRequest<(object, Metadata)>
	{
		public int Page { get; set; }
		public int EachPage { get; set; }
		public string? Search { get; set; }
		public string RoleName { get; set; } = null!;
		public IEnumerable<string>? Status { get; set; } = new List<string>();
    }
}
