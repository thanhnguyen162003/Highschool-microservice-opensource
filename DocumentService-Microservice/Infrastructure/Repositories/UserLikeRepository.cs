using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class UserLikeRepository(DocumentDbContext context) : BaseRepository<UserLike>(context), IUserLikeRepository
{
    public async Task<UserLike?> GetUserLikeAsync(Guid userId)
    {
        return await _entities.FirstOrDefaultAsync(up => up.UserId == userId);
    }
    public async Task<UserLike?> GetUserLikeDocumentAsync(Guid userId, Guid documentId)
    {
        return await _entities.FirstOrDefaultAsync(up => up.UserId == userId && up.DocumentId == documentId);
    }
    public async Task<UserLike?> GetUserLikeFlashcardAsync(Guid userId, Guid flashcardId)
    {
        return await _entities.FirstOrDefaultAsync(up => up.UserId == userId && up.FlashcardId == flashcardId);
    }
    public async Task<UserLike?> GetUserLikeSubjectAsync(Guid userId, Guid subjectId)
    {
        return await _entities.FirstOrDefaultAsync(up => up.UserId == userId && up.SubjectId == subjectId);
    }
    public async Task<UserLike?> GetUserLikeLessonAsync(Guid userId, Guid lessonId)
    {
        return await _entities.FirstOrDefaultAsync(up => up.UserId == userId && up.LessonId == lessonId);
    }
    public async Task<UserLike?> GetUserLikeDocumentNullAsync(Guid userId)
    {
        return await _entities.FirstOrDefaultAsync(up => up.UserId == userId && up.DocumentId == null);
    }
    public async Task<UserLike?> GetUserLikeLessonNullAsync(Guid userId)
    {
        return await _entities.FirstOrDefaultAsync(up => up.UserId == userId && up.LessonId == null);
    }
    public async Task<UserLike?> GetUserLikeSubjectNullAsync(Guid userId)
    {
        return await _entities.FirstOrDefaultAsync(up => up.UserId == userId && up.SubjectId == null);
    }
    public async Task<UserLike?> GetUserLikeFlashcardNullAsync(Guid userId)
    {
        return await _entities.FirstOrDefaultAsync(up => up.UserId == userId && up.FlashcardId == null);
    }
    public async Task<int> GetUserLikeFlashcardAmount(Guid flashcardId)
    {
        return await _entities.Where(up => up.FlashcardId == flashcardId).CountAsync();
    }
    public async Task<bool> CreateUserLike(UserLike quiz, CancellationToken cancellationToken)
    {
        await _entities.AddAsync(quiz, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> UpdateUserLike(UserLike quiz, CancellationToken cancellationToken)
    {
        _entities.Update(quiz);
        var result = await context.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return true;
        }
        return false;
    }

}