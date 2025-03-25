using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionalService.Core.Enumerations;
using TransactionalService.EntityFramework.Core.Common;

namespace TransactionalService.EntityFramework.Core.Entities
{
	//user đăng ký
	public class Subscription : BaseAuditableEntity
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid PlanId { get; set; }
		public required SubscriptionEnum Status { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public virtual Plan Plan { get; set; }
		public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
		public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
	}
}
