using Application.Messages;
using Domain.Models.Common;
using Infrastructure;
using MediatR;
using System.Net;

namespace Application.Features.PlayGame.Command
{
    public class CheckExistCommand : IRequest<APIResponse>
    {
        public string RoomId { get; set; } = string.Empty;
    }

    public class CheckExistCommandHandler : IRequestHandler<CheckExistCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckExistCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<APIResponse> Handle(CheckExistCommand request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.RoomGameRepository.IsExistRoom(request.RoomId) ?
                new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageGame.RoomFound
                } :
                new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageGame.RoomNotFound
                };
        }
    }

}
