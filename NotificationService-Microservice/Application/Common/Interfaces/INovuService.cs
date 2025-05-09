using SharedProjects.ConsumeModel.NovuModel;

namespace Application.Common.Interfaces
{
    public interface INovuService
    {
        Task<bool> TestNotification(string userId);
        Task<bool> CreateSubscriber(NovuSubcriberModel novuSubcriberModel);
        Task<bool> UpdateSubcriber(NovuSubcriberModel novuSubcriberModel);
        Task<bool> CreateTopic(NovuTopicModel novuTopicModel);
        Task<bool> AddUserToTopic(NovuSubcriberTopicModel novuSubcriberTopicModel);
        Task<bool> RemoveUserFromTopic(NovuSubcriberTopicModel novuSubcriberTopicModel);
        Task<bool> TriggerNotification(NovuTriggerNotificationModel model);
        Task<bool> UpdateSubscriberPreferences(string subscriberId, string templateId, bool isEnabled);
    }
}
