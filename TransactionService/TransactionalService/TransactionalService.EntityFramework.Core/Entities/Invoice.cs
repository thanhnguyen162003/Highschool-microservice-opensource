using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionalService.Core.Enumerations;
using TransactionalService.EntityFramework.Core.Common;

namespace TransactionalService.EntityFramework.Core.Entities
{
	//hóa đơn
	public class Invoice : BaseAuditableEntity
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid PaymentId { get; set; }
		public Guid SubscriptionId { get; set; }
		public decimal TotalAmount { get; set; }
		public PaymentStatusEnum Status { get; set; }
		public DateTime IssuedAt { get; set; }
		public DateTime DueDate { get; set; }
		public virtual Payment Payment { get; set; }
		public virtual Subscription Subscription { get; set; }
	}
}
