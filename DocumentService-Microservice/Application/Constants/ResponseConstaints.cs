namespace Application.Constants;

public static class ResponseConstaints
{
	public const string RoleRestriction = "Bạn không có quyền để thực hiện hành động này";

	#region MasterSubject
	public const string MasterSubjectCreated = "Tạo master subject thành công";
    public const string MasterSubjectCreateFailed = "Tạo master subject thất bại";
    public const string MasterSubjectDeleted = "Xóa master subject thành công";
    public const string MasterSubjectDeleteFailed = "Xóa master subject thất bại";
    public const string MasterSubjectUpdated = "Cập nhật master subject thành công";
    public const string MasterSubjectUpdateFailed = "Cập nhật master subject thất bại";
    #endregion

    #region Chapter
    public const string ChapterCreated = "Tạo chương học thành công";
    public const string ChapterCreateFailed = "Tạo chương học thất bại";
    public const string ChapterDeleted = "Xóa chương học thành công";
    public const string ChapterDeleteFailed = "Xóa chương học thất bại";
    public const string ChapterUpdated = "Cập nhật chương học thành công";
    public const string ChapterUpdateFailed = "Cập nhật chương học thất bại";
    #endregion

    #region Curriculum
    public const string CurriculumCreated = "Tạo chương trình học thành công";
    public const string CurriculumCreateFailed = "Tạo chương trình học thất bại";
    public const string CurriculumDeleted = "Xóa chương trình học thành công";
    public const string CurriculumDeleteFailed = "Xóa chương trình học thất bại";
    public const string CurriculumUpdated = "Cập nhật chương trình học thành công";
    public const string CurriculumUpdateFailed = "Cập nhật chương trình học thất bại";
    public const string CurriculumNotFound = "Không thấy chương trình học";
    
    #endregion

    #region Document
    public const string DocumentCreated = "Tạo tài liệu thành công";
    public const string DocumentCreateFailed = "Tạo tài liệu thất bại";
    public const string DocumentDeleted = "Xóa tài liệu thành công";
    public const string DocumentDeleteFailed = "Xóa tài liệu thất bại";
    public const string DocumentUpdated = "Cập nhật tài liệu thành công";
    public const string DocumentUpdateFailed = "Cập nhật tài liệu thất bại";
    #endregion

    #region FlashcardContent
    public const string FlashcardContentCreated = "Tạo nội dung thẻ ghi nhớ thành công";
    public const string FlashcardContentCreateFailed = "Tạo nội dung thẻ ghi nhớ thất bại";
    public const string FlashcardContentDeleted = "Xóa nội dung thẻ ghi nhớ thành công";
    public const string FlashcardContentDeleteFailed = "Xóa nội dung thẻ ghi nhớ thất bại";
    public const string FlashcardContentDeleteFailedBecauseKafka = "Xóa nội dung thẻ ghi nhớ thất bại do kafka produce";
    public const string FlashcardContentDeleteFailedBecauseNull = "Xóa nội dung thẻ ghi nhớ thất bại do null";
    public const string FlashcardContentDeleteFailedDatabase = "Xóa nội dung thẻ ghi nhớ thất bại do database";
    public const string FlashcardContentDeleteFailedDatabaseAdmin = "Xóa nội dung thẻ ghi nhớ thất bại do database (no admin permission)";
    public const string FlashcardContentUpdated = "Cập nhật nội dung thẻ ghi nhớ thành công";
    public const string FlashcardContentUpdateFailed = "Cập nhật nội dung thẻ ghi nhớ thất bại";
    #endregion

    #region Flashcard
    public const string FlashcardCreated = "Tạo thẻ ghi nhớ thành công";
    public const string FlashcardCreateFailed = "Tạo thẻ ghi nhớ thất bại";
    public const string FlashcardDeleted = "Xóa thẻ ghi nhớ thành công";
    public const string FlashcardDeleteFailed = "Xóa thẻ ghi nhớ thất bại";
    public const string FlashcardUpdated = "Cập nhật thẻ ghi nhớ thành công"; 
	public const string FlashcardUpdateFailed = "Cập nhật thẻ ghi nhớ thất bại";
	public const string FlashcardUpdateStatusFailed = "Chỉ có teacher mới có thể thay đổi trạng thái flashcard";
	#endregion

	#region Lesson
	public const string LessonCreated = "Tạo bài học thành công";
    public const string LessonCreateFailed = "Tạo bài học thất bại";
    public const string LessonDeleted = "Xóa bài học thành công";
    public const string LessonDeleteFailed = "Xóa bài học thất bại";
    public const string LessonUpdated = "Cập nhật bài học thành công";
    public const string LessonUpdateFailed = "Cập nhật bài học thất bại";
    #endregion

    #region Question
    public const string QuestionCreated = "Tạo câu hỏi thành công";
    public const string QuestionCreateFailed = "Tạo câu hỏi thất bại";
    public const string QuestionDeleted = "Xóa câu hỏi thành công";
    public const string QuestionDeleteFailed = "Xóa câu hỏi thất bại";
    public const string QuestionUpdated = "Cập nhật câu hỏi thành công";
    public const string QuestionUpdateFailed = "Cập nhật câu hỏi thất bại";
    #endregion

    #region SubjectCurriculum
    public const string SubjectCurriculumCreated = "Tạo chương trình giảng dạy thành công";
    public const string SubjectCurriculumCreateFailed = "Tạo chương trình giảng dạy thất bại";
    public const string SubjectCurriculumPublished = "Phát hành thành công";
    public const string SubjectCurriculumPublishFailed = "Phát hành thất bại";
    public const string SubjectCurriculumUnPublished = "Hủy phát hành thành công";
    public const string SubjectCurriculumUnPublishFailed = "Hủy phát hành thất bại";
    #endregion

    #region Subject
    public const string SubjectCreated = "Tạo môn học thành công";
    public const string SubjectCreateFailed = "Tạo môn học thất bại";
    public const string SubjectDeleted = "Xóa môn học thành công";
    public const string SubjectDeleteFailed = "Xóa môn học thất bại";
    public const string SubjectUpdated = "Cập nhật môn học thành công";
    public const string SubjectUpdateFailed = "Cập nhật môn học thất bại";
    #endregion

    #region Theory
    public const string TheoryCreated = "Tạo lý thuyết thành công";
    public const string TheoryCreateFailed = "Tạo lý thuyết thất bại";
    public const string TheoryDeleted = "Xóa lý thuyết thành công";
    public const string TheoryDeleteFailed = "Xóa lý thuyết thất bại";
    public const string TheoryUpdated = "Cập nhật lý thuyết thành công";
    public const string TheoryUpdateFailed = "Cập nhật lý thuyết thất bại";
    #endregion

    #region StarredTerm
    public const string StarDeleteSuccess = "Bỏ đánh dấu thuật ngữ thành công";
    public const string StarDeleteFailed = "Bỏ đánh dấu thuật ngữ thất bại";
    public const string StarAddSuccess = "Đánh dấu thuật ngữ thành công";
    public const string StarAddFailed = "Đánh dấu thuật ngữ thất bại";
    #endregion
}