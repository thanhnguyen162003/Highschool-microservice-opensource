namespace Infrastructure.Repositories.Interfaces;

public interface IUnitOfWork
{
    ISubjectRepository SubjectRepository { get; }
    IFlashcardRepository FlashcardRepository { get; }
    IChapterRepository ChapterRepository { get; }
    IFlashcardContentRepository FlashcardContentRepository { get; }
    ILessonRepository LessonRepository { get; }
    ITheoryRepository TheoryRepository { get; }
    IDocumentRepository DocumentRepository { get; }
    IMasterSubjectRepository MasterSubjectRepository { get; }
    IEnrollmentProgressRepository EnrollmentProgressRepository { get; }
    IEnrollmentRepository EnrollmentRepository { get; }
    IProvinceRepository ProvinceRepository { get; }
    ISchoolRepository SchoolRepository { get; }
    ICurriculumRepository CurriculumRepository { get; }  
    ISubjectCurriculumRepository SubjectCurriculumRepository { get; }
    IUserFlashcardProgressRepository UserFlashcardProgressRepository { get; }
    IUserLikeRepository UserLikeRepository { get; }
    IQuestionRepository QuestionRepository { get; }
    IQuestionAnswerRepository QuestionAnswerRepository { get; }
    IUserQuizProgressRepository UserQuizProgressRepository { get; }
    IFlashcardFolderRepository FlashcardFolderRepository { get; }
    IFolderUserRepository FolderUserRepository { get; }
    IDocumentFolderRepository DocumentFolderRepository { get; }
    IContainerRepository ContainerRepository { get; }
    IStarredTermRepository StarredTermRepository { get; }
    IStudiableTermRepository StudiableTermRepository { get; }
    ITagRepository TagRepository { get; }
    IFlashcardTagRepository FlashcardTagRepository { get; }
    IFSRSPresetRepository FSRSPresetRepository { get; }
    void SaveChanges();
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}