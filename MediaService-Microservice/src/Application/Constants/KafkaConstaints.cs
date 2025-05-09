namespace Application.Constants;

public class KafkaConstaints
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
    public const string SubjectViewUpdated = "subject_view_updated";
    public const string SubjectImageCreatedRetry = "subject_image_created_retry";
    public const string VideoDeleted = "video_deleted";

    //Media-topic
    public const string SubjectImageUpdated = "subject_image_updated";
    public const string VideoUploadedRetry = "video_uploaded_retry";
    public const string VideoUploaded = "video_uploaded";
    public const string NewViewUpdate = "new_view_updated";
    //User-topic
    public const string RecommendOnboarding = "recommend_onboarding";
    public const string ReportCreated = "report_created";
    public const string UserRegistered = "user_registered";
    
    //Analyse-topic
    public const string UserRoadmapCreated = "user_roadmap_created";
    public const string DataRecommended = "data_recommended";
    public const string UserRoadmapGenCreated = "user_roadmap_gen_created";
    
    //Validation-topic
    public const string SubjectValidation = "subject_validation";
    public const string LessonValidation = "lesson_validation";
    public const string ChapterValidation = "chapter_validation";
    public const string DocumentValidation = "document-validation";

}
