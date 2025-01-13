using Domain.Entities;
using Grpc.Core;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.ExternalService
{
    public class FlashcardService : FlashcardServiceRpc.FlashcardServiceRpcBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FlashcardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<FlashcardResponse> GetFlashcardId(FlashcardRequest request, ServerCallContext context)
        {
            var document = await _unitOfWork.FlashcardRepository.GetFlashcardBySubjectId(request.SubjectId);

            var response = new FlashcardResponse();
            response.FlashcardId.AddRange(document);
            return response;
        }
        public override async Task<FlashcardTipsResponse> GetFlashcardTips(FlashcardTipsRequest request, ServerCallContext context)
        {
            var document = await _unitOfWork.FlashcardRepository.GetFlashcardForTips(request.FlaschcardId);

            var response = new FlashcardTipsResponse();
            foreach (var item in document)
            {
                response.FlaschcardId.Add(item.Id.ToString());
                response.FlaschcardName.Add(item.FlashcardName.ToString());
                response.FlaschcardSlug.Add(item.Slug.ToString());
            }
            return response;
        }
    }
}
