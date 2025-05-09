using MongoDB.Bson;

namespace Application.Common.Models.MBTIModel
{
	public class MBTIQuestionResponseModel
	{
		public string Id { get; set; }
		public string Question { get; set; } = null!;
		public List<MBTIOptionResponseModel> Options { get; set; } = new List<MBTIOptionResponseModel>();
	}
}
