using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.QuestionAnswerModel;
using Application.Common.Models.QuestionModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.QuestionFeature.Commands
{
    public record CreateQuestionCommand : IRequest<ResponseModel>
    {
        public List<QuestionRequestModel> Questions { get; set; } = new List<QuestionRequestModel>();
    }

    public class CreateQuestionCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface) : IRequestHandler<CreateQuestionCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IClaimInterface _claimInterface = claimInterface;

        public async Task<ResponseModel> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra tất cả CategoryId trong danh sách câu hỏi
            foreach (var question in request.Questions)
            {
                var isValidCategory = await ValidateCategoryIdAsync(question, cancellationToken);
                if (!isValidCategory)
                {
                    return new ResponseModel
                    {
                        Status = System.Net.HttpStatusCode.BadRequest,
                        Message = $"Danh mục không hợp lệ hoặc không tồn tại: {question.Category}, ID: {question.CategoryId}"
                    };
                }
            }

            // Bắt đầu giao dịch
            await _unitOfWork.BeginTransactionAsync();

            var questionListResult = new List<Guid>();

            try
            {
                var tasks = request.Questions.Select(async questionRequest =>
                {
                    var questionId = await CreateQuestionAsync(questionRequest, cancellationToken);
                    await CreateAnswersAsync(questionRequest.QuestionAnswers, questionId, cancellationToken);

                    return questionId;
                }).ToList();

                // Chạy các tác vụ đồng thời
                questionListResult.AddRange(await Task.WhenAll(tasks));

                await _unitOfWork.CommitTransactionAsync();

                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.OK,
                    Message = ResponseConstaints.QuestionCreated,
                    Data = questionListResult
                };
            }
            catch (Exception ex)
            {
                questionListResult.Clear();
                await _unitOfWork.RollbackTransactionAsync();

                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.InternalServerError,
                    Message = ResponseConstaints.QuestionCreateFailed,
                    Data = ex.Message
                };
            }
        }


        private async Task<Guid> CreateQuestionAsync(QuestionRequestModel questionRequest, CancellationToken cancellationToken)
        {
            // Kiểm tra xem CategoryId có tồn tại không
            var isValidCategory = await ValidateCategoryIdAsync(questionRequest, cancellationToken);
            if (!isValidCategory)
            {
                throw new Exception($"Danh mục không hợp lệ hoặc không tồn tại: {questionRequest.Category}, ID: {questionRequest.CategoryId}");
            }

            // Tạo đối tượng câu hỏi
            var question = new Question
            {
                Id = new UuidV7().Value,
                QuestionContent = questionRequest.QuestionContent,
                QuestionType = questionRequest.QuestionType,
                Difficulty = questionRequest.Difficulty,
                CreatedBy = _claimInterface.GetCurrentUserId,
                UpdatedBy = _claimInterface.GetCurrentUserId
            };

            // Gán Category ID dựa trên loại câu hỏi
            SetCategoryIds(questionRequest, question);

            // Lưu câu hỏi vào cơ sở dữ liệu
            await _unitOfWork.QuestionRepository.InsertAsync(question);

            return question.Id;
        }

        private async Task<bool> ValidateCategoryIdAsync(QuestionRequestModel questionRequest, CancellationToken cancellationToken)
        {
            switch (questionRequest.Category)
            {
                case QuestionCategory.Lesson:
                    var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(questionRequest.CategoryId, cancellationToken);
                    return lesson != null;

                case QuestionCategory.Chapter:
                    var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(questionRequest.CategoryId, cancellationToken);
                    return chapter != null;

                case QuestionCategory.SubjectCurriculum:
                    var subjectCurriculum = await _unitOfWork.SubjectCurriculumRepository.GetByIdAsync(questionRequest.CategoryId, cancellationToken);
                    return subjectCurriculum != null;

                case QuestionCategory.Subject:
                    var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(questionRequest.CategoryId, cancellationToken);
                    return subject != null;

                default:
                    return false;
            }
        }



        private async Task CreateAnswersAsync(IEnumerable<QuestionAnswerRequestModel> answers, Guid questionId, CancellationToken cancellationToken)
        {
            var answerList = answers.Select(x => new QuestionAnswer
            {
                Id = new UuidV7().Value,
                QuestionId = questionId,
                AnswerContent = x.AnswerContent,
                IsCorrectAnswer = x.IsCorrectAnswer,
                CreatedBy = _claimInterface.GetCurrentUserId,
                UpdatedBy = _claimInterface.GetCurrentUserId
            }).ToList();

            await _unitOfWork.QuestionAnswerRepository.InsertManyAsync(answerList, cancellationToken);
        }

        private void SetCategoryIds(QuestionRequestModel questionRequest, Question question)
        {
            switch (questionRequest.Category)
            {
                case QuestionCategory.Lesson:
                    question.LessonId = questionRequest.CategoryId;
                    break;
                case QuestionCategory.Chapter:
                    question.ChapterId = questionRequest.CategoryId;
                    break;
                case QuestionCategory.SubjectCurriculum:
                    question.SubjectCurriculumId = questionRequest.CategoryId;
                    break;
                case QuestionCategory.Subject:
                    question.SubjectId = questionRequest.CategoryId;
                    break;
            }
        }
    }


}
