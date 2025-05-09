using Application.Messages;
using Application.Services.Authentication;
using AutoMapper;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;
using System.Net;

namespace Application.Features.ZoneFeatures.Commands
{
    public class DeleteZoneCommand : IRequest<APIResponse>
    {
        public Guid ZoneId { get; set; }
    }

    public class DeleteZoneCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService) : IRequestHandler<DeleteZoneCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<APIResponse> Handle(DeleteZoneCommand request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.User.UserId;
            var zone = await _unitOfWork.ZoneRepository.GetById(request.ZoneId);
            if (zone == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.ZoneId
                };
            } else if(!zone.CreatedBy.Equals(userId))
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.Forbidden,
                    Message = MessageCommon.Forbidden,
                };
            }

            zone.DeletedAt = DateTime.UtcNow;
            await _unitOfWork.ZoneRepository.Update(zone);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.DeleteSuccessfully,
                    Data = request.ZoneId
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.DeleteFailed,
                Data = request.ZoneId
            };

        }
    }

}
