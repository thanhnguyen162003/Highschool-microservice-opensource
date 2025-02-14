namespace Application.Common.Models.UserModels
{
    public class UserDetailModel
    {
        public Guid Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Avatar { get; set; }
        public int? TotalPlay { get; set; }
        public int? TotalHost { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Streak { get; set; }
        public int? TotalRank1 { get; set; }
        public int? TotalRank2 { get; set; }
        public int? TotalRank3 { get; set; }
    }
}
