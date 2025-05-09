using MongoDB.Bson;

namespace Application.Common.Models.MBTIModel
{
	public class MBTIResultRequestModel
	{
		public string Id { get; set; }
		public string AnswerOption { get; set; } = null!;
	}
}
