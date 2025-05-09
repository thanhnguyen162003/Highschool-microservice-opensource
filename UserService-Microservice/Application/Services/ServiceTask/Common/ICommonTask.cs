namespace Application.Services.ServiceTask.Common
{
	public interface ICommonTask
	{
		void UpdateInfoBaseUser(Guid userId);
		void PublishUserUpdatedMessage(Guid userId);
	}
}