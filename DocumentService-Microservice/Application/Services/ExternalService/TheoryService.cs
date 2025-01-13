using Domain.Entities;
using Grpc.Core;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.ExternalService
{
    public class TheoryService : TheoryServiceRpc.TheoryServiceRpcBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TheoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public override async Task<TheoryTipsResponse> GetTheoryTips(TheoryTipsRequest request, ServerCallContext context)
        {
            var document = await _unitOfWork.TheoryRepository.GetTheoryForTips(request.TheoryId);

            var response = new TheoryTipsResponse();
            foreach (var item in document)
            {
                response.TheoryId.Add(item.Id.ToString());
                response.TheoryName.Add(item.TheoryName.ToString());
            }
            return response;
        }
    }
}
