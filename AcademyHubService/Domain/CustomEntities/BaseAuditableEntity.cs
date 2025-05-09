namespace Domain.CustomEntities
{
    public class BaseAuditableEntity
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
