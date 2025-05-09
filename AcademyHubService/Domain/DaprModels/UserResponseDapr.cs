using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DaprModels
{
    public class UserResponseDapr
    {
		public string? UserId { get; set; }
		public string? Username { get; set; }
		public string? Role { get; set; }
		public string? Avatar { get; set; }
		public string? Fullname { get; set; }
		public string? Email { get; set; }
	}
}
