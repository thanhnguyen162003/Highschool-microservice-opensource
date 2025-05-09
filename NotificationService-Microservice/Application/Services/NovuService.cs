using Application.Common.Interfaces;
using Newtonsoft.Json;
using Novu;
using Novu.DTO;
using Novu.DTO.Events;
using Novu.DTO.Subscribers;
using Novu.DTO.Subscribers.Preferences;
using Novu.DTO.Topics;
using Novu.Interfaces;
using Novu.Models;
using Novu.Models.Notifications;
using Novu.Models.Subscribers.Preferences;
using Novu.Models.Triggers;
using SharedProjects.ConsumeModel;
using SharedProjects.ConsumeModel.Enums;
using SharedProjects.ConsumeModel.NovuModel;

namespace Application.Services
{
    public class OnboardEventPayload
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
    public class NovuService(IEventClient eventClient, ISubscriberClient subscriberClient, ITopicClient topicClient, IWorkflowClient workflowClient, ILogger<NovuService> logger) : INovuService
    {
        private readonly IEventClient _eventClient = eventClient;
        private readonly ISubscriberClient _subscriberClient = subscriberClient;
        private readonly ITopicClient _topicClient = topicClient;
        private readonly IWorkflowClient _workflowClient = workflowClient;
        private readonly ILogger<NovuService> _logger = logger;

        public async Task<bool> TestNotification(string userId)
        {
            var triggerUserEvent = new Novu.DTO.Events.EventCreateData()
            {
                EventName = "system-workflow",
                To = { SubscriberId = userId, }
            };

            var result = await _eventClient.Trigger(triggerUserEvent);

            if (result?.Data?.Acknowledged ?? false)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> CreateSubscriber(NovuSubcriberModel novuSubcriberModel)
        {
            try
            {
                var newSubcriber = new SubscriberCreateData()
                {
                    SubscriberId = novuSubcriberModel.Id.ToString(),
                    Email = novuSubcriberModel.Email,
                    LastName = novuSubcriberModel.LastName,
                };

                var subscriber = await _subscriberClient.Create(newSubcriber);

                if (subscriber?.Data != null)
                {
                    _logger.LogInformation("NovuInfo: Created subscriber successfully.");
                    return true;
                }
                _logger.LogError("NovuException: Subscriber creation failed.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("NovuException: Create subscriber failed.");
                _logger.LogError($"NovuException: {ex.ToString()}");
                return false;
            }
        }

        public async Task<bool> UpdateSubcriber(NovuSubcriberModel novuSubcriberModel)
        {
            try
            {
                var existingSubscriber = await _subscriberClient.Get(novuSubcriberModel.Id.ToString());

                if (existingSubscriber?.Data == null)
                {
                    _logger.LogError("NovuException: Subscriber not found.");
                    return false;
                }

                existingSubscriber.Data.Email = novuSubcriberModel.Email;
                existingSubscriber.Data.LastName = novuSubcriberModel.LastName;

                var updateSubcriber = new SubscriberEditData()
                {
                    SubscriberId = novuSubcriberModel.Id.ToString(),
                    Email = novuSubcriberModel.Email,
                    LastName = novuSubcriberModel.LastName
                };

                var result = await _subscriberClient.Update(novuSubcriberModel.Id.ToString(), updateSubcriber);

                if (result != null && result.Data != null)
                {
                    _logger.LogInformation("NovuInfo: Update subscriber successfully.");
                    return true;
                }
                _logger.LogError("NovuException: Subcriber is null");
                return false;
            } catch (Exception ex)
            {
                _logger.LogError("NovuException: Update subscriber failed.");
                _logger.LogError($"NovuException: {ex.ToString()}");
                return false;
            }
        }

        public async Task<bool> CreateTopic(NovuTopicModel novuTopicModel)
        {
            try
            {
                var newTopic = new Novu.DTO.Topics.TopicCreateData()
                {
                    Key = novuTopicModel.Key,
                    Name = novuTopicModel.Name,
                };

                var result = await _topicClient.Create(newTopic);

                if (result?.Data != null)
                {
                    _logger.LogInformation("NovuInfo: Create topic successfully.");
                    return true;
                }

                _logger.LogError("NovuException: Topic is null");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("NovuException: Create topic failed.");
                _logger.LogError($"NovuException: {ex.ToString()}");
                return false;
            }
        }
        
        public async Task<bool> AddUserToTopic(NovuSubcriberTopicModel novuSubcriberTopicModel)
        {
            try
            {
                var subscriberUsers = new TopicSubscriberCreateData(novuSubcriberTopicModel.SubcriberIds);

                var result = await _topicClient.AddSubscriber(novuSubcriberTopicModel.TopicKey, subscriberUsers);

                if (result?.Data != null)
                {
                    _logger.LogInformation($"NovuInfo: Add user to topic {novuSubcriberTopicModel.TopicKey} successfully.");
                    return true;
                }

                _logger.LogError($"NovuException: Add user to topic {novuSubcriberTopicModel.TopicKey} failed.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"NovuException: Add user to topic {novuSubcriberTopicModel.TopicKey} failed with exception.");
                _logger.LogError($"NovuException: {ex.ToString()}");
                return false;
            }
        }

        public async Task<bool> RemoveUserFromTopic(NovuSubcriberTopicModel novuSubcriberTopicModel)
        {
            try
            {
                var subscriberUsers = new TopicSubscriberCreateData(novuSubcriberTopicModel.SubcriberIds);

                await _topicClient.RemoveSubscriber(novuSubcriberTopicModel.TopicKey, subscriberUsers);

                _logger.LogInformation($"NovuInfo: Remove user from topic {novuSubcriberTopicModel.TopicKey} successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"NovuException: Remove user from topic {novuSubcriberTopicModel.TopicKey} failed with exception.");
                _logger.LogError($"NovuException: {ex.ToString()}");
                return false;
            }
        }

        public async Task<bool> TriggerNotification(NovuTriggerNotificationModel model)
        {
            try
            {
                NovuResponse<EventAcknowledgeData> result = null;
                switch (model.NotificationTriggerType)
                {
                    case NotificationTriggerType.Users:
                        var triggerUserEvent = new Novu.DTO.Events.EventCreateData()
                        {
                            EventName = model.WorkflowId,
                            To = { SubscriberId = model.TargetId },
                            Payload = model.Payload,
                        };

                        result = await _eventClient.Trigger(triggerUserEvent);

                        break;

                    case NotificationTriggerType.Topic:
                        var triggerTopicEvent = new Novu.DTO.Events.TopicCreateData
                        {
                            EventName = model.WorkflowId,
                            To = [new TopicTrigger(model.TargetId)],
                            Payload = model.Payload
                        };

                        result = await _eventClient.Create(triggerTopicEvent);

                        break;

                    case NotificationTriggerType.SystemWide:
                        result = await _eventClient.CreateBroadcast(new BroadcastEventCreateData()
                        {
                            Name = model.WorkflowId,
                            Payload = model.Payload,
                            
                        });
                        break;
                }

                if (result?.Data?.Acknowledged ?? false)
                {
                    _logger.LogInformation($"NovuInfo: Triggered workflow {model.WorkflowId} for topic {model.TargetId}.");
                    return true;
                }

                _logger.LogError($"NovuException: Failed to trigger workflow {model.WorkflowId}.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"NovuException: Trigger notification failed for workflow {model.WorkflowId}.");
                _logger.LogError($"NovuException: {ex.ToString()}");
                return false;
            }
        }
        public async Task<bool> UpdateSubscriberPreferences(string subscriberId, string templateId, bool isEnabled)
        {
            try
            {
                var result = await _subscriberClient.UpdatePreference(
                    subscriberId,
                    templateId,
                    new SubscriberPreferenceEditData
                    {
                        Channel = new Channel
                        {
                            Type = ChannelTypeEnum.InApp,
                            Enabled = isEnabled
                        },
                    });

                if (result?.Data != null)
                {
                    _logger.LogInformation($"NovuInfo: Updated notification preference for subscriber {subscriberId}.");
                    return true;
                }

                _logger.LogError($"NovuException: Failed to update preference for subscriber {subscriberId}.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"NovuException: Update preference failed for subscriber {subscriberId}.");
                _logger.LogError($"NovuException: {ex.ToString()}");
                return false;
            }
        }


    }
}
