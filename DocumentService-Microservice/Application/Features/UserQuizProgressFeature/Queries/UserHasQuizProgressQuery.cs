using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.QuestionModel;
using Application.Common.Models;
using Infrastructure.Repositories.Interfaces;
using Domain.Enums;
using Domain.Entities;
using System.Linq.Expressions;
using Application.Common.Models.UserQuizProgress;

namespace Application.Features.UserQuizProgressFeature.Queries
{
    public record UserHasQuizProgressQuery : IRequest<ResponseModel>
    {
        public GetQuizRequestModel RequestModel { get; set; } = new GetQuizRequestModel();
    }
    public class UserHasQuizProgressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claimInterface) : IRequestHandler<UserHasQuizProgressQuery, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IClaimInterface _claimInterface = claimInterface;

        public async Task<ResponseModel> Handle(UserHasQuizProgressQuery request, CancellationToken cancellationToken)
        {
            var model = request.RequestModel;

            // Validate CategoryId
            var isValidCategory = await ValidateCategoryIdAsync(model.QuestionCategory, model.CategoryId, cancellationToken);
            if (!isValidCategory)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.BadRequest,
                    Message = $"Danh mục '{model.QuestionCategory}' với ID '{model.CategoryId}' không hợp lệ hoặc không tồn tại."
                };
            }

            var userQuizProgress = await GetUserQuizProgressAsync(_claimInterface.GetCurrentUserId, model.QuestionCategory, model.CategoryId, cancellationToken);
            if (userQuizProgress != null)
            {
                return new ResponseModel
                {
                    Data = new HasProgressResponseModel()
                    {
                        HasProgress = true,
                    },
                    Message = "Bạn đã có tiến trình ôn tập cho mục này",
                    Status = System.Net.HttpStatusCode.OK,
                };
            }
            else
            {
                return new ResponseModel
                {
                    Data = new HasProgressResponseModel()
                    {
                        HasProgress = false,
                    },
                    Message = "Bạn chưa có tiến trình ôn tập cho mục này",
                    Status = System.Net.HttpStatusCode.OK,
                };
            }
        }

        private async Task<UserQuizProgress?> GetUserQuizProgressAsync(Guid userId, QuestionCategory category, Guid categoryId, CancellationToken cancellationToken)
        {
            // Check the category type and build the predicate accordingly
            Expression<Func<UserQuizProgress, bool>> predicate;

            switch (category)
            {
                case QuestionCategory.Lesson:
                    predicate = uqp => uqp.UserId == userId && uqp.LessonId == categoryId;
                    break;
                case QuestionCategory.Chapter:
                    predicate = uqp => uqp.UserId == userId && uqp.ChapterId == categoryId;
                    break;
                case QuestionCategory.SubjectCurriculum:
                    predicate = uqp => uqp.UserId == userId && uqp.SubjectCurriculumId == categoryId;
                    break;
                case QuestionCategory.Subject:
                    predicate = uqp => uqp.UserId == userId && uqp.SubjectId == categoryId;
                    break;
                default:
                    throw new ArgumentException("Invalid category", nameof(category));
            }

            // Get user progress using the repository's Get method and return the first match if any
            var userProgressList = await _unitOfWork.UserQuizProgressRepository.Get(
                filter: predicate,
                cancellationToken: cancellationToken
            );

            return userProgressList.FirstOrDefault();
        }

        private async Task<bool> ValidateCategoryIdAsync(
            QuestionCategory category,
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            return category switch
            {
                QuestionCategory.Lesson => await _unitOfWork.LessonRepository
                    .GetQueryable(q => q.Id == categoryId)
                    .AnyAsync(cancellationToken),

                QuestionCategory.Chapter => await _unitOfWork.ChapterRepository
                    .GetQueryable(q => q.Id == categoryId)
                    .AnyAsync(cancellationToken),

                QuestionCategory.SubjectCurriculum => await _unitOfWork.SubjectCurriculumRepository
                    .GetQueryable(q => q.Id == categoryId)
                    .AnyAsync(cancellationToken),

                QuestionCategory.Subject => await _unitOfWork.SubjectRepository
                    .GetQueryable(q => q.Id == categoryId)
                    .AnyAsync(cancellationToken),

                _ => false
            };
        }
    }
}
