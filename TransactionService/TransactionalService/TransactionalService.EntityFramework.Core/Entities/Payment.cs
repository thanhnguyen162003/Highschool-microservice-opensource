using System;
using System.Collections;
using TransactionalService.Core.Enumerations;
using TransactionalService.EntityFramework.Core.Common;

namespace TransactionalService.EntityFramework.Core.Entities
{
	public class Payment: BaseAuditableEntity
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid SubscriptionId { get; set; }
		public string PaymentMethod { get; set; }
		public required decimal Amount { get; set; }
		public required CurrencyEnum Currency { get; set; }
		public PaymentStatusEnum? Status { get; set; }
		public string TransactionId { get; set; }
		public virtual Subscription Subscription { get; set; }
	}
}
