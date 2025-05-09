using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Constants.Services
{
    public class TopicKafkaConstaints
    {
        public const string MailZoneCreated = "mail_zone_created";
        public const string MailCreated = "mail_created";

        //notification-topic
        public const string NotificationDocumentCreated = "notification_document_created";
        public const string NotificationUserCreated = "notification_user_created";
        public const string NotificationNewsCreated = "notification_news_created";
        public const string NotificationSystemCreated = "notification_system_created";
        public const string NotificationCreateSubscriber = "notification_create_subscriber";
        public const string NotificationUpdateSubscriber = "notification_update_subscriber";
        public const string NotificationCreateTopic = "notification_create_topic";
        public const string NotificationAddUserToTopic = "notification_add_user_to_topic";
        public const string NotificationRemoveUserFromTopic = "notification_remove_user_from_topic";
        public const string NotificationTriggerNotification = "notification_trigger_notification";

    }
}
