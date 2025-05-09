namespace Infrastructure.Repositories.Interfaces;

public interface IUserLikeRepository
{
    Task<UserLike?> GetUserLikeAsync(Guid userId);
    Task<bool> CreateUserLike(UserLike quiz, CancellationToken cancellationToken);
    Task<bool> UpdateUserLike(UserLike quiz, CancellationToken cancellationToken);
    Task<UserLike?> GetUserLikeDocumentAsync(Guid userId, Guid documentId);
    Task<UserLike?> GetUserLikeFlashcardAsync(Guid userId, Guid flashcardId);
    Task<UserLike?> GetUserLikeSubjectAsync(Guid userId, Guid subjectId);
    Task<UserLike?> GetUserLikeLessonAsync(Guid userId, Guid lessonId);
    Task<UserLike?> GetUserLikeDocumentNullAsync(Guid userId);
    Task<UserLike?> GetUserLikeLessonNullAsync(Guid userId);
    Task<UserLike?> GetUserLikeSubjectNullAsync(Guid userId);
    Task<UserLike?> GetUserLikeFlashcardNullAsync(Guid userId);
    Task<int> GetUserLikeFlashcardAmount(Guid flashcardId);
}