using Application.Common.Models;
using Application.Common.Models.FlashcardFeatureModel;

namespace Application.Common.Interfaces;

public interface IFlashcardStudyService
{
    Task<StudyProgressModel> GetStudyProgress(Guid userId, Guid flashcardId);
    Task<List<FlashcardContentQuestionModel>> GenerateFlashcardQuestionsPublic(Guid flashcardId, int questionCount = 7);
    Task<ResponseModel> UpdateUserProgress(Guid userId, Guid flashcardContentId, Guid flashcardId, bool isCorrect);
    Task<FlashcardQuestionResponseModel> GenerateFlashcardQuestions(Guid flashcardId, Guid userId, int questionCount = 7);
    Task<ResponseModel> ResetProgress(Guid userId, Guid flashcardId);
}