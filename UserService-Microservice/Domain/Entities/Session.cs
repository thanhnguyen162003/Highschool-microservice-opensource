using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Session : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? DeviceInfo { get; set; }
        public string? IpAddress { get; set; }
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpiredAt { get; set; }
        public bool IsRevoked { get; set; }

        public virtual BaseUser? User { get; set; }
    }
}
