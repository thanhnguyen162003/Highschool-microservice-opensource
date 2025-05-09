using Application.Common.Models.QuestionModel;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq.Expressions;

namespace Application.Features.QuestionFeature.Queries
{
    public record GetQuizQuery : IRequest<ResponseModel>
    {
        public GetQuizRequestModel RequestModel { get; set; } = new GetQuizRequestModel();
    }
    public class GetQuizQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claimInterface) : IRequestHandler<GetQuizQuery, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IClaimInterface _claimInterface = claimInterface;

        public async Task<ResponseModel> Handle(GetQuizQuery request, CancellationToken cancellationToken)
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

            // Determine total questions
            var totalQuestions = GlobalConstant.NumberOfQuestionsFromCategory[model.QuestionCategory];
            var difficultyDistribution = GlobalConstant.PercentOfQuestionsFromDifficulty
                .Select(pair => new { pair.Key, Count = (int)Math.Round(pair.Value * totalQuestions / 100.0) })
                .ToDictionary(x => x.Key, x => x.Count);

            // Fetch User's Progress
            var userProgress = await GetUserQuizProgressAsync(_claimInterface.GetCurrentUserId, model.QuestionCategory, model.CategoryId, cancellationToken);
            if (userProgress == null)
            {
                // If no progress found, return a default set of difficulty distribution
                return await GenerateQuizResponse(difficultyDistribution, totalQuestions);
            }

            // Calculate user progress score
            var totalProgress = CalculateUserProgress(userProgress);

            // Adjust difficulty distribution based on user progress
            difficultyDistribution = AdjustDifficultyDistribution(difficultyDistribution, totalProgress);

            // Fetch questions recursively by category
            var selectedQuestions = await GetQuestions(
                model.QuestionCategory,
                model.CategoryId,
                totalQuestions,
                difficultyDistribution,
                cancellationToken);

            // Check if we have enough questions
            if (selectedQuestions.Count < totalQuestions)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.ServiceUnavailable,
                    Message = "Không có đủ câu hỏi trong ngân hàng đề. Bạn hãy quay lại sau."
                };
            }

            // Convert to response models and mask correct answers
            var mappedQuestions = _mapper.Map<List<QuestionResponseModel>>(selectedQuestions);

            // Create response
            var response = new QuizResponseModel { Questions = mappedQuestions };

            return new ResponseModel
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Lấy bài quiz thành công.",
                Data = response
            };
        }

        private async Task<ResponseModel> GenerateQuizResponse(
    Dictionary<Difficulty, int> difficultyDistribution,
    int totalQuestions)
        {
            var selectedQuestions = new List<Question>();

            // Lọc câu hỏi từ ngân hàng câu hỏi theo độ khó và chọn ngẫu nhiên
            foreach (var difficulty in difficultyDistribution.Keys)
            {
                // Lấy câu hỏi từ repository theo độ khó đã phân phối và chọn ngẫu nhiên
                var questions = await _unitOfWork.QuestionRepository.Get(
                    q => q.Difficulty == difficulty,
                    orderBy: q => q.OrderBy(x => Guid.NewGuid()), // Lấy ngẫu nhiên
                    takeCount: difficultyDistribution[difficulty],
                    includeProperties: q => q.QuestionAnswers
                );

                // Thêm câu hỏi vào danh sách đã chọn
                selectedQuestions.AddRange(questions);
            }

            // Nếu số lượng câu hỏi không đủ, trả về lỗi
            if (selectedQuestions.Count < totalQuestions)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.ServiceUnavailable,
                    Message = "Không có đủ câu hỏi trong ngân hàng đề. Bạn hãy quay lại sau."
                };
            }

            // Sử dụng AutoMapper để chuyển đổi danh sách câu hỏi thành mô hình phản hồi
            var mappedQuestions = _mapper.Map<List<QuestionResponseModel>>(selectedQuestions);

            // Tạo và trả về phản hồi quiz
            var response = new QuizResponseModel { Questions = mappedQuestions };

            return new ResponseModel
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Lấy bài quiz thành công.",
                Data = response
            };
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


        private async Task<List<Question>> GetQuestions(
            QuestionCategory category,
            Guid categoryId,
            int totalQuestions,
            Dictionary<Difficulty, int> difficultyDistribution,
            CancellationToken cancellationToken)
        {
            var selectedQuestions = new List<Question>();

            // Lọc câu hỏi từ ngân hàng dựa trên category
            var currentLevelQuestions = await GetQuestionsByCategory(category, categoryId, cancellationToken);

            // Lấy câu hỏi theo phân phối độ khó đã điều chỉnh
            foreach (var difficulty in difficultyDistribution.Keys)
            {
                var availableQuestions = currentLevelQuestions
                    .Where(q => q.Difficulty == difficulty)
                    .OrderBy(_ => Guid.NewGuid()) // Ngẫu nhiên hóa câu hỏi
                    .Take(difficultyDistribution[difficulty])
                    .ToList();

                selectedQuestions.AddRange(availableQuestions);
                difficultyDistribution[difficulty] -= availableQuestions.Count;
            }

            // Nếu đã đủ số lượng câu hỏi, trả về
            if (selectedQuestions.Count >= totalQuestions || category == QuestionCategory.Lesson)
                return selectedQuestions;

            var lessonLevelQuestions = await GetQuestionByLessonChild(category, categoryId, cancellationToken);

            foreach (var difficulty in difficultyDistribution.Keys)
            {
                var availableQuestions = lessonLevelQuestions
                    .Where(q => q.Difficulty == difficulty)
                    .OrderBy(_ => Guid.NewGuid()) // Ngẫu nhiên hóa câu hỏi
                    .Take(difficultyDistribution[difficulty])
                    .ToList();

                selectedQuestions.AddRange(availableQuestions);
                difficultyDistribution[difficulty] -= availableQuestions.Count;
            }

            if (selectedQuestions.Count >= totalQuestions || category == QuestionCategory.Chapter)
                return selectedQuestions;

            var chapterLevelQuestions = await GetQuestionByChapterChild(category, categoryId, cancellationToken);

            foreach (var difficulty in difficultyDistribution.Keys)
            {
                var availableQuestions = chapterLevelQuestions
                    .Where(q => q.Difficulty == difficulty)
                    .OrderBy(_ => Guid.NewGuid()) // Ngẫu nhiên hóa câu hỏi
                    .Take(difficultyDistribution[difficulty])
                    .ToList();

                selectedQuestions.AddRange(availableQuestions);
                difficultyDistribution[difficulty] -= availableQuestions.Count;
            }

            if (selectedQuestions.Count >= totalQuestions || category == QuestionCategory.SubjectCurriculum)
                return selectedQuestions;

            var subjectCurriculumLevelQuestions = await GetQuestionBySubjectCurriculumChild(category, categoryId, cancellationToken);

            foreach (var difficulty in difficultyDistribution.Keys)
            {
                var availableQuestions = subjectCurriculumLevelQuestions
                    .Where(q => q.Difficulty == difficulty)
                    .OrderBy(_ => Guid.NewGuid()) // Ngẫu nhiên hóa câu hỏi
                    .Take(difficultyDistribution[difficulty])
                    .ToList();

                selectedQuestions.AddRange(availableQuestions);
                difficultyDistribution[difficulty] -= availableQuestions.Count;
            }

            return selectedQuestions;
        }

        private async Task<List<Question>> GetQuestionByLessonChild(QuestionCategory category, Guid categoryId, CancellationToken cancellationToken)
        {
            IEnumerable<Question> returnList = new List<Question>();
            switch (category)
            {
                case QuestionCategory.Subject:
                    returnList = await _unitOfWork.QuestionRepository.Get(q => q.LessonId != null && q.Lesson!.Chapter.SubjectCurriculum.Subject!.Id == categoryId, includeProperties: x => x.QuestionAnswers);
                    break;

                case QuestionCategory.SubjectCurriculum:
                    returnList = await _unitOfWork.QuestionRepository.Get(q => q.LessonId != null && q.Lesson!.Chapter.SubjectCurriculum.Id == categoryId, includeProperties: x => x.QuestionAnswers);
                    break;

                case QuestionCategory.Chapter:
                    returnList = await _unitOfWork.QuestionRepository.Get(q => q.LessonId != null && q.Lesson!.Chapter.Id == categoryId, includeProperties: x => x.QuestionAnswers);
                    break;
            }

            return returnList.ToList();
        }

        private async Task<List<Question>> GetQuestionByChapterChild(QuestionCategory category, Guid categoryId, CancellationToken cancellationToken)
        {
            IEnumerable<Question> returnList = new List<Question>();
            switch (category)
            {
                case QuestionCategory.Subject:
                    returnList = await _unitOfWork.QuestionRepository.Get(q => q.ChapterId != null && q.Chapter!.SubjectCurriculum.Subject!.Id == categoryId, includeProperties: x => x.QuestionAnswers);
                    break;

                case QuestionCategory.SubjectCurriculum:
                    returnList = await _unitOfWork.QuestionRepository.Get(q => q.ChapterId != null && q.Chapter!.SubjectCurriculum.Id == categoryId, includeProperties: x => x.QuestionAnswers);
                    break;
            }

            return returnList.ToList();
        }

        private async Task<List<Question>> GetQuestionBySubjectCurriculumChild(QuestionCategory category, Guid categoryId, CancellationToken cancellationToken)
        {
            IEnumerable<Question> returnList = new List<Question>();
            switch (category)
            {
                case QuestionCategory.Subject:
                    returnList = await _unitOfWork.QuestionRepository.Get(q => q.SubjectCurriculumId != null && q.SubjectCurriculum!.Subject!.Id == categoryId, includeProperties: x => x.QuestionAnswers);
                    break;
            }

            return returnList.ToList();
        }

        private async Task<List<Question>> GetQuestionsByCategory(
            QuestionCategory category,
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.QuestionRepository
                .Get(q => GetCategoryPredicate(q, category, categoryId), cancellationToken: cancellationToken, includeProperties: x => x.QuestionAnswers);

            return list.ToList();
        }

        private bool GetCategoryPredicate(Question question, QuestionCategory category, Guid categoryId)
        {
            return category switch
            {
                QuestionCategory.Subject => question.SubjectId == categoryId,
                QuestionCategory.SubjectCurriculum => question.SubjectCurriculumId == categoryId,
                QuestionCategory.Chapter => question.ChapterId == categoryId,
                QuestionCategory.Lesson => question.LessonId == categoryId,
                _ => false
            };
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

        private float CalculateUserProgress(UserQuizProgress userProgress)
        {
            // Tính toán tiến độ dựa trên các phần trăm không phải null
            var totalFields = new List<float?>
        {
            userProgress.RecognizingPercent,
            userProgress.ComprehensingPercent,
            userProgress.LowLevelApplicationPercent,
            userProgress.HighLevelApplicationPercent
        };

            // Lọc ra các giá trị không null và tính tiến độ trung bình
            var validPercentages = totalFields.Where(p => p.HasValue).Select(p => p.Value).ToList();

            if (validPercentages.Any())
            {
                var sum = validPercentages.Sum();
                return sum / validPercentages.Count;
            }

            return 0; // Nếu không có phần trăm hợp lệ nào, trả về tiến độ là 0
        }

        private Dictionary<Difficulty, int> AdjustDifficultyDistribution(Dictionary<Difficulty, int> difficultyDistribution, float totalProgress)
        {
            // Điều chỉnh phân phối câu hỏi dựa trên tiến độ của người dùng
            if (totalProgress >= 0.85f)
            {
                // Điều chỉnh đều hơn cho tất cả các mức độ khó
                difficultyDistribution[Difficulty.LowLevelApplication] += (int)(difficultyDistribution[Difficulty.LowLevelApplication] * 0.2);
                difficultyDistribution[Difficulty.HighLevelApplication] += (int)(difficultyDistribution[Difficulty.Comprehensing] * 0.2);
                difficultyDistribution[Difficulty.Recognizing] -= (int)(difficultyDistribution[Difficulty.LowLevelApplication] * 0.2);
                difficultyDistribution[Difficulty.Comprehensing] -= (int)(difficultyDistribution[Difficulty.LowLevelApplication] * 0.2);              
            }
            else if (totalProgress >= 0.5f)
            {
                // Điều chỉnh đều hơn cho tất cả các mức độ khó
                difficultyDistribution[Difficulty.Recognizing] += (int)(difficultyDistribution[Difficulty.LowLevelApplication] * 0.2);
                difficultyDistribution[Difficulty.Comprehensing] += (int)(difficultyDistribution[Difficulty.LowLevelApplication] * 0.1);
                difficultyDistribution[Difficulty.LowLevelApplication] -= (int)(difficultyDistribution[Difficulty.LowLevelApplication] * 0.1);
                difficultyDistribution[Difficulty.HighLevelApplication] -= (int)(difficultyDistribution[Difficulty.HighLevelApplication] * 0.1);
            }
            else
            {
                // Điều chỉnh đều hơn cho tất cả các mức độ khó
                difficultyDistribution[Difficulty.Recognizing] += (int)(difficultyDistribution[Difficulty.LowLevelApplication] * 0.3);
                difficultyDistribution[Difficulty.Comprehensing] += (int)(difficultyDistribution[Difficulty.LowLevelApplication] * 0.3);
                difficultyDistribution[Difficulty.LowLevelApplication] -= (int)(difficultyDistribution[Difficulty.LowLevelApplication] * 0.2);           
                difficultyDistribution[Difficulty.HighLevelApplication] -= (int)(difficultyDistribution[Difficulty.Comprehensing] * 0.2);
            }

            // Đảm bảo không có phần trăm nào dưới 5%
            foreach (var key in difficultyDistribution.Keys.ToList())
            {
                if (difficultyDistribution[key] < 5)
                {
                    difficultyDistribution[key] = 5;
                }
            }

            return difficultyDistribution;
        }

    }
}
