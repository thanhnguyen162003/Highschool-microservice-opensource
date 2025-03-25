using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionalService.Core.Enumerations;
using TransactionalService.EntityFramework.Core.Common;

namespace TransactionalService.EntityFramework.Core.Entities
{
	//gói cước
	public class Plan : BaseAuditableEntity
	{
		public Guid Id { get; set; }
		public required string PlanName { get; set; }
		public required decimal PlanPrice { get; set; }
		public required CurrencyEnum Currency { get; set; }
		public required int Duration { get; set; }
		public bool IsActive { get; set; }
		public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
	}
}
