using Application.Common.Interfaces;
using Application.Common.Models.FlashcardTestModel;
using Application.Constants;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services;

public class FlashcardTestService(IUnitOfWork unitOfWork) : IFlashcardTestService
{
    public async Task<FlashcardTestQuestionModel> GenerateFlashcardTest(Guid flashcardId, TestOptionModel option)
    {
        var random = new Random();

        var flashcardContents =
            await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(flashcardId);
        if (!flashcardContents.Any())
        {
            return new FlashcardTestQuestionModel
            {
                Questions = new List<FlashcardContentTestQuestionModel>()
            };
        }

        int questionCount = option.NumberQuestion ?? 10;

        var selectedContents = flashcardContents
            .OrderBy(_ => random.Next())
            .Take(questionCount)
            .ToList();
        var questions = new List<FlashcardContentTestQuestionModel>();
        if (option.TypeTest.Equals(TypeTestConstraint.MULTIPLECHOICE))
        {
            foreach (var correctTerm in selectedContents)
            {
                var incorrectDefinitions = flashcardContents
                    .Where(content => content.Id != correctTerm.Id)
                    .OrderBy(_ => random.Next())
                    .Take(3)
                    .ToList();

                if (incorrectDefinitions.Count < 3) continue;
                
                var options = incorrectDefinitions
                    .Select(content => new FlashcardContentTestQuestionModel.AnswerOption
                    {
                        // Id = content.Id,
                        Definition = content.FlashcardContentDefinition
                    })
                    .ToList();

                options.Add(new FlashcardContentTestQuestionModel.AnswerOption
                {
                    // Id = correctTerm.Id,
                    Definition = correctTerm.FlashcardContentDefinition
                });

                options = options.OrderBy(_ => random.Next()).ToList();

                questions.Add(new FlashcardContentTestQuestionModel()
                {
                    Term = correctTerm.FlashcardContentTerm,
                    FlashcardContentId = correctTerm.Id,
                    Options = options,
                });
            }
        }
        return new FlashcardTestQuestionModel
        {
            Questions = questions
        };
    }
    
    public async Task<FlashcardContentTrueFalseResponse> GenerateFlashcardTestTrueFalse(Guid flashcardId, TestOptionModel option)
    {
        if (flashcardId == Guid.Empty)
        {
            throw new ArgumentException("FlashcardId cannot be empty.", nameof(flashcardId));
        }

        if (option == null)
        {
            throw new ArgumentNullException(nameof(option), "Test options must be provided.");
        }

        var random = new Random();

        var flashcardContents = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(flashcardId);
        if (!flashcardContents.Any())
        {
            return new FlashcardContentTrueFalseResponse
            {
                Questions = new List<FlashcardContentTrueFalseQuestionModel>()
            };
        }

        int questionCount = option.NumberQuestion ?? 10;
        
        var selectedContents = flashcardContents
            .OrderBy(_ => random.Next())
            .Take(questionCount)
            .ToList();

        var questions = new List<FlashcardContentTrueFalseQuestionModel>();
        foreach (var term in selectedContents)
        {
            var useCorrectDefinition = random.Next(0, 2) == 0;

            var presentedDefinition = useCorrectDefinition
                ? term.FlashcardContentDefinition
                : flashcardContents
                    .Where(content => content.Id != term.Id)
                    .OrderBy(_ => random.Next())
                    .Select(content => content.FlashcardContentDefinition)
                    .FirstOrDefault();

            questions.Add(new FlashcardContentTrueFalseQuestionModel
            {
                Term = term.FlashcardContentTerm,
                FlashcardContentId = term.Id,
                PresentedDefinition = presentedDefinition ?? string.Empty,
                IsCorrect = useCorrectDefinition
            });
        }

        return new FlashcardContentTrueFalseResponse
        {
            Questions = questions
        };
    }

    
    public async Task<FlashcardTestResultModel> SubmitAnswers(Guid flashcardId, List<FlashcardAnswerSubmissionModel> answers)
    {
        var flashcardContents = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(flashcardId);
        if (!flashcardContents.Any())
        {
            return new FlashcardTestResultModel
            {
                CorrectCount = 0,
                WrongCount = 0,
                Percentage = 0,
                AnswerResults = new List<FlashcardAnswerResultModel>()
            };
        }

        var correctAnswerDictionary = flashcardContents
            .ToDictionary(content => content.Id, content => content.FlashcardContentDefinition);

        var answerResults = new List<FlashcardAnswerResultModel>();
        int correctCount = 0;
        int wrongCount = 0;

        foreach (var answer in answers)
        {
            var isCorrect = correctAnswerDictionary.TryGetValue(answer.FlashcardContentId, out var correctAnswer)
                            && string.Equals(correctAnswer, answer.SubmittedAnswer, StringComparison.OrdinalIgnoreCase);

            if (isCorrect)
            {
                correctCount++;
            }
            else
            {
                wrongCount++;
            }

            answerResults.Add(new FlashcardAnswerResultModel
            {
                Term = flashcardContents.First(content => content.Id == answer.FlashcardContentId).FlashcardContentTerm,
                FlashcardContentId = answer.FlashcardContentId,
                SubmittedAnswer = answer.SubmittedAnswer,
                IsCorrect = isCorrect,
                CorrectAnswer = correctAnswer
            });
        }
        
        var totalAnswers = correctCount + wrongCount;
        var percentage = totalAnswers > 0 ? ((double)correctCount / totalAnswers) * 100 : 0;

        return new FlashcardTestResultModel
        {
            CorrectCount = correctCount,
            WrongCount = wrongCount,
            Percentage = percentage,
            AnswerResults = answerResults
        };
    }
    
    public async Task<FlashcardTrueFalseTestResultModel> SubmitTrueFalseAnswers(Guid flashcardId, List<TrueFalseAnswerSubmissionModel> answers)
    {
        var flashcardContents = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(flashcardId);
        if (!flashcardContents.Any())
        {
            return new FlashcardTrueFalseTestResultModel
            {
                CorrectCount = 0,
                WrongCount = 0,
                Percentage = 0,
                AnswerResults = new List<TrueFalseAnswerResultModel>()
            };
        }

        var answerResults = new List<TrueFalseAnswerResultModel>();
        int correctCount = 0;
        int wrongCount = 0;

        foreach (var answer in answers)
        {
            var content = flashcardContents.FirstOrDefault(c => c.Id == answer.FlashcardContentId);
            if (content == null) continue;

            var isCorrectAnswer = content.FlashcardContentDefinition == answer.SelectedOption.ToString();
            if (isCorrectAnswer) correctCount++; else wrongCount++;

            answerResults.Add(new TrueFalseAnswerResultModel
            {
                Term = content.FlashcardContentTerm,
                FlashcardContentId = content.Id,
                SelectedOption = answer.SelectedOption,
                IsCorrect = isCorrectAnswer
            });
        }

        var totalAnswers = correctCount + wrongCount;
        var percentage = totalAnswers > 0 ? ((double)correctCount / totalAnswers) * 100 : 0;

        return new FlashcardTrueFalseTestResultModel
        {
            CorrectCount = correctCount,
            WrongCount = wrongCount,
            Percentage = percentage,
            AnswerResults = answerResults
        };
    }


}
