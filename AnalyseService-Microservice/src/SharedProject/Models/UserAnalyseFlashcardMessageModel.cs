namespace SharedProject.Models;

public class UserAnalyseFlashcardMessageModel
{
    // Thông tin cơ bản về sự kiện
    public Guid EventId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime TimeStamp { get; set; }
    public Guid? SessionId { get; set; }  // Theo dõi phiên học tập
    public UserAnalyseDataModel? DataStudyMode { get; set; }
    public UserAnalyseDataModelLearning? DataLearningMode { get; set; }
    public string? StudyContext { get; set; }  // Bối cảnh học tập: exam_prep, daily_review, etc.
    public string? StudyMode { get; set; } //Learn , Test , Flaschard .if learn, test use the UserAnalyseDataModel and if Flashcard use the UserAnalyseDataModelLearning
    public TimeSpan? TimeOfDay { get; set; }     
    public DayOfWeek? DayOfWeek { get; set; } 
}

public class UserAnalyseDataModel
{
    public Guid? FlashcardId { get; set; }
    public List<Guid>? FlashcardContentIdRight { get; set; }
    public List<Guid>? FlashcardContentIdFalse { get; set; }
    public long? TimeSpentMs { get; set; }  // Thời gian dành cho phiên học
    public int? AnswerCorrect { get; set; }  // Câu trả lời đúng hay sai
    public int? TotalQuestionsInRound { get; set; }
    public long? AverageAnswerTimeEachQuestionMs { get; set; }  // Thời gian để trả lời
    public int? ViewCount { get; set; }  // Số lần xem flashcard này
    public bool? IsReview { get; set; }  // Đây là lần học mới hay ôn tập
    public int? CardsStudiedInSession { get; set; }  // Số flashcard đã học trong phiên
    public int? CorrectAnswersInSession { get; set; }  // Số câu trả lời đúng trong phiên
    public double? AccuracyRate { get; set; }  // Tỷ lệ chính xác (0-1)
    public DateTime? LastReviewDate { get; set; }  // Lần ôn tập trước đó
    public DateTime? NextScheduledReview { get; set; }  // Lần ôn tập tiếp theo được lên lịch
    public int? RepetitionNumber { get; set; }  // Số lần ôn tập flashcard này
    public double? EaseFactor { get; set; }  // Hệ số dễ dàng (trong thuật toán SM-2)
    public int? IntervalDays { get; set; }  // Khoảng thời gian giữa các lần ôn tập (ngày)
}
public class UserAnalyseDataModelLearning
{
    public Guid? FlashcardId { get; set; }
    public List<Guid>? FlashcardContentIdRight { get; set; }
    public List<Guid>? FlashcardContentIdFalse { get; set; }
    public long? TimeSpentMs { get; set; }  // Thời gian dành cho phiên học
    public int? SelfRating { get; set; }  // Tự đánh giá (1-5)
    public long? AverageAnswerTimeEachCardMs { get; set; }  // Thời gian để trả lời
    public double? AccuracyRate { get; set; }  // Tỷ lệ chính xác (0-1)
    public int? FlipCount { get; set; }            // Số lần lật thẻ trong một lần xem
}
