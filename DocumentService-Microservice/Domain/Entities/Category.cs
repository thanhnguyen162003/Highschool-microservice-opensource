namespace Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public string CategorySlug { get; set; } = null!;
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
