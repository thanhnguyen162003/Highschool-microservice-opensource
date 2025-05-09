using Application.Common.Models.QuestionAnswerModel;
using Application.Common.Models;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.UserQuizProgress;
using Domain.Entities;
using Domain.Enums;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Application.Common.Models.QuestionModel;

namespace Application.Features.QuestionFeature.Commands
{
    public record SubmitQuizAnswerCommand : IRequest<ResponseModel>
    {
        public SubmitAnswerRequestModel RequestModel { get; set; } = new SubmitAnswerRequestModel();
    }
    public class SubmitQuizAnswerCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface, IMapper mapper) : IRequestHandler<SubmitQuizAnswerCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IClaimInterface _claimInterface = claimInterface;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(SubmitQuizAnswerCommand request, CancellationToken cancellationToken)
        {
            var model = request.RequestModel;

            // Validate CategoryId
            var isValidCategory = await ValidateCategoryIdAsync(model.QuestionCategory, model.CategoryId, cancellationToken);
            if (!isValidCategory)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.BadRequest,
                    Message = "Danh mục không hợp lệ hoặc không tồn tại."
                };
            }

            // Fetch questions and calculate progress
            var questionIds = model.Answers.Select(a => a.QuestionId).ToList();
            var questions = await _unitOfWork.QuestionRepository.Get(
                q => questionIds.Contains(q.Id),
                includeProperties: q => q.QuestionAnswers);

            if (questions.Count() != model.Answers.Count)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.BadRequest,
                    Message = "Một số câu hỏi không tồn tại."
                };
            }

            var correctCounts = new Dictionary<Difficulty, int>
            {
                { Difficulty.Recognizing, 0 },
                { Difficulty.Comprehensing, 0 },
                { Difficulty.LowLevelApplication, 0 },
                { Difficulty.HighLevelApplication, 0 }
            };

            var questionCounts = new Dictionary<Difficulty, int>
            {
                { Difficulty.Recognizing, 0 },
                { Difficulty.Comprehensing, 0 },
                { Difficulty.LowLevelApplication, 0 },
                { Difficulty.HighLevelApplication, 0 }
            };

            foreach (var answer in model.Answers)
            {
                var question = questions.First(q => q.Id == answer.QuestionId);
                questionCounts[question.Difficulty]++;

                // Check correctness
                var correctAnswerIds = question.QuestionAnswers
                    .Where(qa => qa.IsCorrectAnswer)
                    .Select(qa => qa.Id)
                    .ToList();

                var isCorrect = question.QuestionType switch
                {
                    QuestionType.SingleChoice => answer.QuestionAnswerIds.Count == 1 && correctAnswerIds.Contains(answer.QuestionAnswerIds.First()),
                    QuestionType.MultipleChoice => !correctAnswerIds.Except(answer.QuestionAnswerIds).Any() && !answer.QuestionAnswerIds.Except(correctAnswerIds).Any(),
                    _ => false
                };

                if (isCorrect)
                {
                    correctCounts[question.Difficulty]++;
                }
            }

            // Calculate percentages
            var recognizingPercent = questionCounts[Difficulty.Recognizing] > 0
                ? correctCounts[Difficulty.Recognizing] * 100f / questionCounts[Difficulty.Recognizing]
                : (float?)null;

            var comprehensingPercent = questionCounts[Difficulty.Comprehensing] > 0
                ? correctCounts[Difficulty.Comprehensing] * 100f / questionCounts[Difficulty.Comprehensing]
                : (float?)null;

            var lowLevelPercent = questionCounts[Difficulty.LowLevelApplication] > 0
                ? correctCounts[Difficulty.LowLevelApplication] * 100f / questionCounts[Difficulty.LowLevelApplication]
                : (float?)null;

            var highLevelPercent = questionCounts[Difficulty.HighLevelApplication] > 0
                ? correctCounts[Difficulty.HighLevelApplication] * 100f / questionCounts[Difficulty.HighLevelApplication]
                : (float?)null;

            // Save progress
            var progress = new UserQuizProgress
            {
                Id = Guid.NewGuid(),
                UserId = _claimInterface.GetCurrentUserId,
                LessonId = model.QuestionCategory == QuestionCategory.Lesson ? model.CategoryId : null,
                ChapterId = model.QuestionCategory == QuestionCategory.Chapter ? model.CategoryId : null,
                SubjectCurriculumId = model.QuestionCategory == QuestionCategory.SubjectCurriculum ? model.CategoryId : null,
                SubjectId = model.QuestionCategory == QuestionCategory.Subject ? model.CategoryId : null,
                QuestionIds = model.Answers.Select(a => a.QuestionId.ToString()).ToList(),
                RecognizingQuestionQuantity = questionCounts[Difficulty.Recognizing],
                RecognizingPercent = recognizingPercent,
                ComprehensingQuestionQuantity = questionCounts[Difficulty.Comprehensing],
                ComprehensingPercent = comprehensingPercent,
                LowLevelApplicationQuestionQuantity = questionCounts[Difficulty.LowLevelApplication],
                LowLevelApplicationPercent = lowLevelPercent,
                HighLevelApplicationQuestionQuantity = questionCounts[Difficulty.HighLevelApplication],
                HighLevelApplicationPercent = highLevelPercent,
            };

            _unitOfWork.UserQuizProgressRepository.Insert(progress);
            await _unitOfWork.SaveChangesAsync();

            // Map to response
            var response = _mapper.Map<UserQuizProgressResponseModel>(progress);
            response.Questions = _mapper.Map<List<QuestionResponseModel>>(questions).OrderBy(q => q.Difficulty).ToList();

            return new ResponseModel
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Bài quiz đã được nộp thành công.",
                Data = response
            };
        }

        private async Task<bool> ValidateCategoryIdAsync(QuestionCategory category, Guid categoryId, CancellationToken cancellationToken)
        {
            return category switch
            {
                QuestionCategory.Lesson => await _unitOfWork.LessonRepository.GetByIdAsync(categoryId, cancellationToken) != null,
                QuestionCategory.Chapter => await _unitOfWork.ChapterRepository.GetByIdAsync(categoryId, cancellationToken) != null,
                QuestionCategory.SubjectCurriculum => await _unitOfWork.SubjectCurriculumRepository.GetByIdAsync(categoryId, cancellationToken) != null,
                QuestionCategory.Subject => await _unitOfWork.SubjectRepository.GetByIdAsync(categoryId, cancellationToken) != null,
                _ => false
            };
        }
    }
}
