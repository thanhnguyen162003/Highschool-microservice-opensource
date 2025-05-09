namespace Application.Constants;

public static class TopicKafkaConstaints
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
    public const string SubjectViewUpdate = "subject_view_updated";
    public const string DocumentViewUpdate = "document_view_updated";
    public const string DocumentFileUpdatedRetry = "document_file_updated_retry";
    //Media-topic
    public const string SubjectImageUpdated = "subject_image_updated";
    public const string SubjectImageUpdatedRetry = "subject_image_updated_retry";
    public const string VideoUploaded = "video_uploaded";
    public const string VideoUploadedRetry = "video_uploaded_retry";
    
    //User-topic
    public const string RecommendOnboarding = "recommend_onboarding";
	public const string MajorSelected = "major_selected";
	public const string ReportCreated = "report_created";
	public const string ReportDocumentCreated = "report_document_created";
	public const string UserRegistered = "user_registered";
    public const string MailCreated = "mail_created";
    public const string MailZoneCreated = "mail_zone_created";
    public const string MailCreatedRetry = "mail_created_retry";
    public const string MailCreatedForgotPassword = "mail_created_forgot_password";
    public const string UserRecommnedRoadmapMissed = "user_recommend_roadmap_missed";
    public const string UserRecommnedDataMissed = "user_recommend_data_missed";
    public const string ProgressStageUpdated = "progress_stage_updated";
    
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

    //GroupId

}