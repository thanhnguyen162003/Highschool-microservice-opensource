namespace Application.Common.Models.HollandModel
{
    public class HollandResultRequestModel
    {
        public string Id { get; set; }
        public List<string> AnswerOption { get; set; } = new List<string>();
    }
}
