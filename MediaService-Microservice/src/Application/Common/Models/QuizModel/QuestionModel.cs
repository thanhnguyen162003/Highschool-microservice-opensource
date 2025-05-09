namespace Application.Common.Models.QuizModel;

public class QuestionModel
{
    public string? AnswerA { get; set; }

    public string? AnswerB { get; set; }

    public string? AnswerC { get; set; }

    public string? AnswerD { get; set; }

    public string? CorrectAnswer { get; set; }

    public string? Question { get; set; }

    public override string ToString()
    {
        return 
            "{" +
            "\n AnswerA: string," +
            "\n AnswerB: string," +
            "\n AnswerC: string," +
            "\n AnswerD: string," +
            "\n CorrectAnswer: string (Full content of answer)," +
            "\n Question: string" +
            "\n}";
    }
}
