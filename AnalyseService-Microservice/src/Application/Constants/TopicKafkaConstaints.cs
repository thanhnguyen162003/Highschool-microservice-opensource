namespace Application.Constants;

public static class TopicKafkaConstaints
{
    public const string SubjectCreated = "subject_created";
    public const string LessonCreated = "lesson_created";
    public const string ChapterCreated = "chapter_created";
    public const string SubjectImageCreated = "subject_image_created";
    public const string SubjectDeleted = "subject_deleted";
    public const string DocumentFileUpdated = "document_file_updated";
    public const string DocumentDeleted = "document_deleted";
    public const string TheoryDeleted = "theory_deleted";
    public const string UserAnalyseData = "user_analyse_data";
    public const string SubjectViewUpdate = "subject_view_update";
    public const string RecentViewCreated = "recent_view_created";
    
    //Media-topic
    public const string SubjectImageUpdated = "subject_image_updated";
    
    //User-topic
    public const string RecommendOnboarding = "recommend_onboarding";
    public const string ReportCreated = "report_created";
    public const string UserRegistered = "user_registered";
    public const string UserRecommnedRoadmapMissed = "user_recommend_roadmap_missed";
    public const string UserRecommnedDataMissed = "user_recommend_data_missed";

    //Analyse-topic
    public const string RecentViewDeleted = "recent-view-deleted";
    public const string UserRoadmapCreated = "user_roadmap_created";
    public const string DataRecommended = "data_recommended";
    public const string UserRoadmapGenCreated = "user_roadmap_gen_created";
    public const string RecommendOnboardingRetry = "recommend_onboarding_retry";
    public const string RecommendOnboardingRetryRoadmapGen = "recommend_onboarding_retry_roadmap_gen";
    public const string RecommendOnboardingDeadLetter = "recommend_onboarding_dead_letter";
    public const string RecommendOnboardingDeadLetterRoadmapGen = "recommend_onboarding_dead_letter_roadmap_gen";
    
    //Validation-topic
    public const string SubjectValidation = "subject_validation";
    public const string LessonValidation = "lesson_validation";
    public const string ChapterValidation = "chapter_validation";
    public const string DocumentValidation = "document-validation";
    
    //GroupId

}
