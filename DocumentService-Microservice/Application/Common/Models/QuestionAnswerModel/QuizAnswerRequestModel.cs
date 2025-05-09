namespace Application.Common.Models.QuestionAnswerModel
{
    public class QuizAnswerRequestModel
    {
        public Guid QuestionId { get; set; }
        public List<Guid> QuestionAnswerIds { get; set; } = new List<Guid>();
    }

}
