//using Domain.Entities;
//using Grpc.Core;
//using Infrastructure.Repositories.Interfaces;
//using System.Reflection.Metadata;

//namespace Application.Services.ExternalService
//{
//    public class FlashcardService(IUnitOfWork unitOfWork) : FlashcardServiceRpc.FlashcardServiceRpcBase
//    {
//        public override async Task<FlashcardResponse> GetFlashcardId(FlashcardRequest request, ServerCallContext context)
//        {
//            var document = await unitOfWork.FlashcardRepository.GetFlashcardBySubjectId(request.SubjectId);

//            var response = new FlashcardResponse();
//            response.FlashcardId.AddRange(document);
//            return response;
//        }
//        public override async Task<UserFlashcardLearningResponse> GetUserFlashcardLearning(UserFlashcardLearningRequest request, ServerCallContext context)
//        {
//            var progress = await unitOfWork.UserFlashcardProgressRepository.GetAllProgressLearning();
//            var response = new UserFlashcardLearningResponse()
//            {
//                UserFlashcardLearning =
//                {
//                    progress.Select(p => new UserFlashcardLearning
//                    {
//                        UserId = p.UserId.ToString(),
//                        FlashcardId = p.FlashcardId.ToString(), // Ensure FlashcardContent is loaded
//                        FlashcardContentId = p.FlashcardContentId.ToString(),
//                        LastReviewDateHistory = { p.LastReviewDateHistory.Select(date => date.ToString("o")) },
//                        TimeSpentHistory = { p.TimeSpentHistory }
//                    })
//                }
//            };
//            return response;
//        }
//        public override async Task<FlashcardTipsResponse> GetFlashcardTips(FlashcardTipsRequest request, ServerCallContext context)
//        {
//            var flashcards = await unitOfWork.FlashcardRepository.GetFlashcardForTips(request.FlaschcardId);
//            if (flashcards == null || !flashcards.Any())
//            {
//                return new FlashcardTipsResponse(); // Return an empty response if no data is found.
//            }
//            var response = new FlashcardTipsResponse()
//            {
//                FlaschcardId = {flashcards.Select(f => f.Id.ToString()) },
//                FlaschcardName = { flashcards.Select(f => f.FlashcardName) },
//                FlaschcardSlug = { flashcards.Select(f => f.Slug) },
//            };
//            return response;
//        }
//    }
//}
