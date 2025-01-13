namespace Domain.Models.KetModels
{
    public class KetContentRequestModel
    {
        public IEnumerable<string> Answers { get; set; } = new List<string>();

        public int? CorrectAnswer { get; set; }

        public string? Question { get; set; }

        public int? TimeAnswer { get; set; }

        public int? Order { get; set; }
    }
}
