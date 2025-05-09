using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.QuestionModel;
using Application.Common.Models.UserQuizProgress;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.UserQuizProgressFeature.Commands
{
    public record ClearUserQuizProgressCommand : IRequest<ResponseModel>
    {
        public ClearUserQuizProgressRequestModel Model { get; set; }
    }

    public class ClearUserQuizProgressCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface) : IRequestHandler<ClearUserQuizProgressCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IClaimInterface _claimInterface = claimInterface;

        public async Task<ResponseModel> Handle(ClearUserQuizProgressCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra tất cả CategoryId trong danh sách câu hỏi
            var isValidCategory = await ValidateCategoryIdAsync(request.Model, cancellationToken);
            if (!isValidCategory)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.BadRequest,
                    Message = $"Danh mục không hợp lệ hoặc không tồn tại: {request.Model.Category}, ID: {request.Model.CategoryId}"
                };
            }

            // Bắt đầu giao dịch
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                switch (request.Model.Category)
                {
                    case QuestionCategory.Lesson:
                        await _unitOfWork.UserQuizProgressRepository.DeleteManyAsync(x => x.LessonId == request.Model.CategoryId && x.UserId == _claimInterface.GetCurrentUserId, cancellationToken);
                        break;

                    case QuestionCategory.Chapter:
                        await _unitOfWork.UserQuizProgressRepository.DeleteManyAsync(x => x.ChapterId == request.Model.CategoryId && x.UserId == _claimInterface.GetCurrentUserId, cancellationToken);
                        break;

                    case QuestionCategory.SubjectCurriculum:
                        await _unitOfWork.UserQuizProgressRepository.DeleteManyAsync(x => x.SubjectCurriculumId == request.Model.CategoryId && x.UserId == _claimInterface.GetCurrentUserId, cancellationToken);
                        break;

                    case QuestionCategory.Subject:
                        await _unitOfWork.UserQuizProgressRepository.DeleteManyAsync(x => x.SubjectId == request.Model.CategoryId && x.UserId == _claimInterface.GetCurrentUserId, cancellationToken);
                        break;
                }

                await _unitOfWork.CommitTransactionAsync();

                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.NoContent,
                    Message = "Xóa tiến trình thành công."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.InternalServerError,
                    Message = "Đã xảy ra lỗi khi xóa tiến trình.",
                    Data = ex.Message
                };
            }
        }

        private async Task<bool> ValidateCategoryIdAsync(ClearUserQuizProgressRequestModel request, CancellationToken cancellationToken)
        {
            switch (request.Category)
            {
                case QuestionCategory.Lesson:
                    var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.CategoryId, cancellationToken);
                    return lesson != null;

                case QuestionCategory.Chapter:
                    var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(request.CategoryId, cancellationToken);
                    return chapter != null;

                case QuestionCategory.SubjectCurriculum:
                    var subjectCurriculum = await _unitOfWork.SubjectCurriculumRepository.GetByIdAsync(request.CategoryId, cancellationToken);
                    return subjectCurriculum != null;

                case QuestionCategory.Subject:
                    var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(request.CategoryId, cancellationToken);
                    return subject != null;

                default:
                    return false;
            }
        }
    }
}
