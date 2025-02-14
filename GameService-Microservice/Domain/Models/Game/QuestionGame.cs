namespace Domain.Models.PlayGameModels
{
    public class QuestionGame
    {
        public Guid? Id { get; set; }

        public IEnumerable<string> Answers { get; set; } = new List<string>();

        public int? CorrectAnswer { get; set; }

        public string? Question { get; set; }

        public int? Order { get; set; }

        public int? TimeAnswer { get; set; }
    }
}
