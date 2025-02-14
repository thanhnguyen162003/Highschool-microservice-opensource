using AutoMapper;
using Application.Messages;
using Domain.Models.Common;
using Domain.Models.PlayGameModels;
using Infrastructure;
using MediatR;
using System.Net;

namespace Application.Features.PlayGame.Command
{
    public class SaveDataGameCommand : IRequest<APIResponse>
    {
        public string RoomId { get; set; } = string.Empty;
        public IEnumerable<ResultPlayerModel> Players { get; set; } = new List<ResultPlayerModel>();
    }

    public class TimeOverQuestionCommandHandler : IRequestHandler<SaveDataGameCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TimeOverQuestionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse> Handle(SaveDataGameCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransaction();

            var players = _mapper.Map<IEnumerable<PlayerGame>>(request.Players);
            foreach (var player in players)
            {
                await _unitOfWork.LeaderboardRepository.Update(player);
            }


            if (await _unitOfWork.CommitTransaction())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.UpdateSuccesfully
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.UpdateFailed
            };
        }
    }

}
