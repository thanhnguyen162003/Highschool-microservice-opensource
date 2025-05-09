
namespace Domain.DaprModels
{
	public class CheckResourceExistsResponseDapr
	{
		public ResourceCheckResultDapr Result { get; set; }
	}
	public class ResourceCheckResultDapr
	{
		public string ResourceId { get; set; }
		public bool Exists { get; set; }
		public string ResourceType { get; set; }
	}
}
