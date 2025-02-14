namespace Domain.Models.PlayGameModels
{
    public class ResultPlayerModel
    {
        public Guid? UserId { get; set; }

        public string? Avatar { get; set; }

        public string? DisplayName { get; set; }

        public int? Score { get; set; }

        public int? Rank { get; set; }

        public int? TimeAverage { get; set; }

    }
}
