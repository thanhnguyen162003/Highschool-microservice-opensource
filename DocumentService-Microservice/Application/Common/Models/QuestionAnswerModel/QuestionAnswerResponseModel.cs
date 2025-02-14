namespace Application.Common.Models.QuestionAnswerModel
{
    public class QuestionAnswerResponseModel
    {
        public Guid Id { get; set; }
        public string AnswerContent { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
