using Application.Common.Models.UserModels;
using Application.Messages;
using Application.Services.Authentication;
using AutoMapper;
using Domain.Entity;
using Domain.Models.Common;
using Infrastructure;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Application.Features.UserFeatures.Query
{
    public class GetUserDetailQuery : IRequest<APIResponse>
    {
    }

    public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public GetUserDetailQueryHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        public async Task<APIResponse> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.GetUserId();

            var user = await _unitOfWork.UserRepository.GetById(userId);
            var history = await _unitOfWork.HistoryPlayRepository.GetAll(h => h.UserId.Equals(userId));

            if(user == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound
                };
            }

            var userDetailModel = _mapper.Map<UserDetailModel>(user);
            if(!history.IsNullOrEmpty())
            {
                userDetailModel.TotalRank1 = history.Count(h => h.Rank == 1);
                userDetailModel.TotalRank2 = history.Count(h => h.Rank == 2);
                userDetailModel.TotalRank3 = history.Count(h => h.Rank == 3);
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.OK,
                Message = MessageCommon.GetSuccesfully,
                Data = userDetailModel
            };
        }


    }

}
