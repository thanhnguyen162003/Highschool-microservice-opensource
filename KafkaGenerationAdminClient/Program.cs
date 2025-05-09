using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace KafkaTopicCreator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bootstrapServers = "";
            var saslUsername = "";
            var saslPassword = "";

            var config = new AdminClientConfig
            {
                BootstrapServers = bootstrapServers,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = saslUsername,
                SaslPassword = saslPassword
            };

            var topicsWithThreePartitions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "user_analyse_data",
                "mail_zone_created",
                "recent_view_created",
                "recommend_onboarding",
                "user_registered",
                "mail_created",
                "mail_created_retry",
                "mail_created_forgot_password",
                "data_recommended",
                "notification_document_created",
                "notification_user_created",
                "notification_news_created",
                "notification_system_created",
                "notification_create_subscriber",
                "notification_update_subscriber",
                "notification_create_topic",
                "notification_add_user_to_topic",
                "notification_remove_user_from_topic",
                "notification_trigger_notification",
                "flashcard-notifications",
                "notification_flashcard_ai_gen",
                "recent-view-deleted"
            };

            var topicsWithOneDayRetention = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "subject_view_updated",
                "document_view_updated",
                "new_view_updated",
                "progress_stage_updated"
            };

            var allTopics = new List<string>
            {
                "subject_created",
                "mail_zone_created",
                "lesson_created",
                "chapter_created",
                "subject_image_created",
                "subject_deleted",
                "lesson_deleted",
                "document_file_updated",
                "data_search_modified",
                "document_deleted",
                "theory_deleted",
                "user_analyse_data",
                "subject_view_updated",
                "document_file_updated_retry",
                "subject_image_updated_retry",
                "document_view_updated",
                "recent_view_created",
                "flashcard_view_updated",
                "flashcard_vote_updated",
                "flashcard_comment_created",
                "subject_image_updated",
                "video_uploaded",
                "video_deleted",
                "video_uploaded_retry",
                "subject_image_created_retry",
                "new_view_updated",
                "recommend_onboarding",
                "report_created",
                "user_registered",
                "user_roadmap_gen_created_retry",
                "mail_created",
                "mail_created_retry",
                "mail_created_forgot_password",
                "user_recommend_roadmap_missed",
                "user_recommend_data_missed",
                "progress_stage_updated",
                "report_document_created",
                "major_selected",
                "user_roadmap_created",
                "flashcard_analyze_data",
                "data_recommended",
                "recommend_onboarding_dead_letter",
                "recommend_onboarding_dead_letter_roadmap_gen",
                "user_roadmap_gen_created",
                "recommend_onboarding_retry",
                "recommend_onboarding_retry_roadmap_gen",
                "notification_document_created",
                "notification_user_created",
                "notification_news_created",
                "notification_system_created",
                "notification_create_subscriber",
                "notification_update_subscriber",
                "notification_create_topic",
                "notification_add_user_to_topic",
                "notification_remove_user_from_topic",
                "notification_trigger_notification",
                "flashcard-notifications",
                "notification_flashcard_ai_gen",
                "recent-view-deleted",
            };

            using var adminClient = new AdminClientBuilder(config).Build();

            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet(StringComparer.OrdinalIgnoreCase);

                var topicsToCreate = allTopics
                    .Where(topic => !existingTopics.Contains(topic))
                    .Select(topic => new TopicSpecification
                    {
                        Name = topic,
                        NumPartitions = topicsWithThreePartitions.Contains(topic) ? 3 : 1,
                        ReplicationFactor = 3,
                        Configs = topicsWithOneDayRetention.Contains(topic)
                            ? new Dictionary<string, string> { { "retention.ms", "86400000" } }
                            : null
                    })
                    .ToList();

                if (topicsToCreate.Count == 0)
                {
                    Console.WriteLine("✅ All topics already exist. Nothing to create.");
                    return;
                }

                await adminClient.CreateTopicsAsync(topicsToCreate);
                Console.WriteLine($"✅ Successfully created {topicsToCreate.Count} topics.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }
}
