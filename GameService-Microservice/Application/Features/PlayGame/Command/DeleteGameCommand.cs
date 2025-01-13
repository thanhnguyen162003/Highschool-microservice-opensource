using Application.Messages;
using Domain.Models.Common;
using Infrastructure;
using MediatR;
using System.Net;

namespace Application.Features.PlayGame.Command
{
    public class DeleteGameCommand : IRequest<APIResponse>
    {
        public string Id { get; set; } = string.Empty;
    }

    public class DeleteGameCommandHandler : IRequestHandler<DeleteGameCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteGameCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<APIResponse> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransaction();

            await _unitOfWork.RoomGameRepository.Delete(request.Id);
            await _unitOfWork.QuestionGameRepository.DeleteQuestion(request.Id);
            await _unitOfWork.LeaderboardRepository.DeletePlayer(request.Id);

            if (await _unitOfWork.CommitTransaction())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageGame.DeleteGameSuccess,
                    Data = request.Id
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageGame.NetworkError
            };
        }
    }

}
