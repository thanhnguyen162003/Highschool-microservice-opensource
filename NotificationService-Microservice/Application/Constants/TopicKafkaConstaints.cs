namespace Application.Constants;

public class TopicKafkaConstaints
{
    //Document-topic
    public const string SubjectCreated = "subject_created";
    public const string LessonCreated = "lesson_created";
    public const string ChapterCreated = "chapter_created";
    public const string SubjectImageCreated = "subject_image_created";
    public const string SubjectDeleted = "subject_deleted";
    public const string DocumentFileUpdated = "document_file_updated";
    public const string DocumentDeleted = "document_deleted";
    public const string TheoryDeleted = "theory_deleted";
    public const string UserAnalyseData = "user_analyse_data";
    
    //Media-topic
    public const string SubjectImageUpdated = "subject_image_updated";
    public const string VideoUploaded = "video_uploaded";
    
    //User-topic
    public const string RecommendOnboarding = "recommend_onboarding";
    public const string ReportCreated = "report_created";
    public const string UserRegistered = "user_registered";
    
    //Analyse-topic
    public const string UserRoadmapCreated = "user_roadmap_created";
    public const string DataRecommended = "data_recommended";
    
    //Validation-topic
    public const string SubjectValidation = "subject_validation";
    public const string LessonValidation = "lesson_validation";
    public const string ChapterValidation = "chapter_validation";
    public const string DocumentValidation = "document-validation";
    
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
    public const string FlashcardDueNotification = "flashcard-notifications";
    public const string NotificationFlashcardAIGen = "notification_flashcard_ai_gen";
}