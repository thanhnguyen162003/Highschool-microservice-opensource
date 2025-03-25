using System;
using TransactionalService.EntityFramework.Core.Common;

namespace TransactionalService.EntityFramework.Core.Entities
{
    public class FeatureUsage : BaseAuditableEntity
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string FeatureKey { get; set; }  // Must match FeatureQuota.FeatureKey
		public DateTime UsageDate { get; set; } // For tracking daily usage
		public int UsageCount { get; set; }     // Increments with each use
	}
}
