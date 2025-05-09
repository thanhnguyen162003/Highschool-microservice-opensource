//using Domain.Entities;
//using Grpc.Core;
//using Infrastructure.Repositories.Interfaces;

//namespace Application.Services.ExternalService
//{
//    public class TheoryService(IUnitOfWork unitOfWork) : TheoryServiceRpc.TheoryServiceRpcBase
//    {
//        public override async Task<TheoryTipsResponse> GetTheoryTips(TheoryTipsRequest request, ServerCallContext context)
//        {
//            var document = await unitOfWork.TheoryRepository.GetTheoryForTips(request.TheoryId);

//            var response = new TheoryTipsResponse();
//            foreach (var item in document)
//            {
//                response.TheoryId.Add(item.Id.ToString());
//                response.TheoryName.Add(item.TheoryName.ToString());
//            }
//            return response;
//        }
//    }
//}
