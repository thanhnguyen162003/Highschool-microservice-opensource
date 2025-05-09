using Application.Common.Models.FlashcardTestModel;
using Domain.CustomModel;

namespace Application.Common.Interfaces;

public interface IFlashcardTestService
{
    Task<FlashcardTestQuestionModel> GenerateFlashcardTest(Guid flashcardId, TestOptionModel option);
    Task<FlashcardTestResultModel> SubmitAnswers(Guid flashcardId, List<FlashcardAnswerSubmissionModel> answers);
    Task<FlashcardTrueFalseTestResultModel> SubmitTrueFalseAnswers(Guid flashcardId, List<TrueFalseAnswerSubmissionModel> answers);
    Task<FlashcardContentTrueFalseResponse> GenerateFlashcardTestTrueFalse(Guid flashcardId, TestOptionModel option);
}
