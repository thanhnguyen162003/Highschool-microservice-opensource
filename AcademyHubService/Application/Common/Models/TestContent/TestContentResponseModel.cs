using Domain.Entity;

namespace Application.Common.Models.TestContent
{
    public class TestContentResponseModel
    {
        public Guid Id { get; set; }

        public Guid? Assignmentid { get; set; }

        public List<string>? Answers { get; set; }

        public int? CorrectAnswer { get; set; }

        public string? Question { get; set; }

        public int? Order { get; set; }

        //public double? Score { get; set; }

    }
}
