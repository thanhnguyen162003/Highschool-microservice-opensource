using Application.Common.Models.QuestionAnswerModel;
using Application.Common.Models;
using Domain.Enums;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.UUID;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Application.Common.Models.QuestionModel;
using Application.Constants;

namespace Application.Features.QuestionFeature.Commands
{
    public record UpdateQuestionCommand : IRequest<ResponseModel>
    {
        public Guid QuestionId { get; set; }
        public QuestionRequestModel Question { get; set; }
    }
    public class UpdateQuestionCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface) : IRequestHandler<UpdateQuestionCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IClaimInterface _claimInterface = claimInterface;

        public async Task<ResponseModel> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            // Lấy câu hỏi từ database
            var question = await _unitOfWork.QuestionRepository.GetByIdAsync(request.QuestionId, cancellationToken);

            if (question == null)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Câu hỏi không tồn tại."
                };
            }

            // Kiểm tra CategoryId có hợp lệ không
            var isValidCategory = await ValidateCategoryIdAsync(request.Question.Category, request.Question.CategoryId, cancellationToken);
            if (!isValidCategory)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.BadRequest,
                    Message = "Danh mục không hợp lệ hoặc không tồn tại."
                };
            }

            // Cập nhật thông tin câu hỏi
            question.QuestionContent = request.Question.QuestionContent;
            question.QuestionType = request.Question.QuestionType;
            question.Difficulty = request.Question.Difficulty;
            question.UpdatedBy = _claimInterface.GetCurrentUserId;

            SetCategoryIds(request, question);

            // Lấy các câu trả lời hiện có
            var existingAnswers = await _unitOfWork.QuestionAnswerRepository.Get(
                x => x.QuestionId == request.QuestionId);

            // Xóa các câu trả lời cũ
            await _unitOfWork.QuestionAnswerRepository.DeleteManyAsync(x => x.QuestionId == request.QuestionId, cancellationToken);

            // Thêm các câu trả lời mới
            var newAnswers = request.Question.QuestionAnswers.Select(x => new QuestionAnswer
            {
                Id = new UuidV7().Value,
                QuestionId = question.Id,
                AnswerContent = x.AnswerContent,
                IsCorrectAnswer = x.IsCorrectAnswer,
                CreatedBy = _claimInterface.GetCurrentUserId,
                UpdatedBy = _claimInterface.GetCurrentUserId
            }).ToList();

            await _unitOfWork.QuestionAnswerRepository.InsertManyAsync(newAnswers, cancellationToken);

            // Lưu thay đổi
            await _unitOfWork.SaveChangesAsync();

            return new ResponseModel
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = ResponseConstaints.QuestionUpdated
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
                _ => false,
            };
        }

        private void SetCategoryIds(UpdateQuestionCommand request, Question question)
        {
            question.LessonId = request.Question.Category == QuestionCategory.Lesson ? request.Question.CategoryId : null;
            question.ChapterId = request.Question.Category == QuestionCategory.Chapter ? request.Question.CategoryId : null;
            question.SubjectCurriculumId = request.Question.Category == QuestionCategory.SubjectCurriculum ? request.Question.CategoryId : null;
            question.SubjectId = request.Question.Category == QuestionCategory.Subject ? request.Question.CategoryId : null;
        }
    }

}
