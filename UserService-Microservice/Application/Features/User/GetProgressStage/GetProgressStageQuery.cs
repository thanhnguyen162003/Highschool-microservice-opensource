using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.User.GetProgressStage
{
    public class GetProgressStageQuery : IRequest<ResponseModel>
    {
    }

    public class GetProgressStageQueryHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService) : IRequestHandler<GetProgressStageQuery, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<ResponseModel> Handle(GetProgressStageQuery request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.GetUserId();

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

            if(user == null)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageUser.UserNotFound
                };
            }

            return new ResponseModel()
            {
                Status = HttpStatusCode.OK,
                Message = MessageCommon.GetSuccess,
                Data = user.ProgressStage
            };
        }
    }
}
