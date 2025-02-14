using Application.Common.Helper;
using Application.Messages;
using Application.Services.Authentication;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Common;
using Domain.Models.Game;
using Domain.Models.PlayGameModels;
using Infrastructure;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Application.Features.PlayGame.Command
{
    public class CreateRoomCommand : IRequest<APIResponse>
    {
        public Guid KetId { get; set; }
    }

    public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public CreateRoomCommandHandler(IUnitOfWork unitOfWork,
                            IAuthenticationService authenticationService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        public async Task<APIResponse> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.GetUserId();
            var ket = await _unitOfWork.KetRepository.GetMyKet(request.KetId, userId);

            if (ket == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.KetId
                };
            } else if (ket.KetContents.IsNullOrEmpty())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageGame.KetContentEmpty
                };
            }

            var room = new RoomGame()
            {
                Id = Utils.GenerateRandomNumber(),
                KetId = request.KetId,
                UserId = userId,
                RoomStatus = RoomStatus.Waiting,
                TotalQuestion = ket.TotalQuestion ?? 0,
                Name = ket.Name,
                Thumbnail = ket.Thumbnail
            };

            var questions = _mapper.Map<IEnumerable<QuestionGame>>(ket.KetContents);

            try
            {
                await _unitOfWork.RoomGameRepository.Add(room);
                await _unitOfWork.QuestionGameRepository.AddQuestions(room.Id, questions);

                return new APIResponse()
                {
                    Status = HttpStatusCode.Created,
                    Message = MessageGame.CreaetRoomSuccess,
                    Data = room
                };
            } catch (Exception ex)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = MessageGame.CreateRoomFail,
                    Data = ex.Message
                };
            }


        }


    }
}
