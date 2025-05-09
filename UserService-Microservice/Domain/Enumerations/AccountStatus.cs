using System.ComponentModel;

namespace Domain.Enumerations
{
	public enum AccountStatus
	{
		[Description("All")]
		All = 0,
		[Description("Active")]
		Active = 1,
		[Description("Pending")]
		Pending = 2,
		[Description("Blocked")]
		Blocked = 3,
		[Description("Deleted")]
		Deleted = 4

	}
}
