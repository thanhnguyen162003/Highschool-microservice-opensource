using Application.Messages;
using Domain.Enums;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.ZoneFeatures.Commands
{
    public class ChangeStatusZoneCommand : IRequest<APIResponse>
    {
        [JsonIgnore]
        public Guid? Id { get; set; }
        public ZoneStatusEnum Status { get; set; }
    }

    public class ChangeStatusZoneCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangeStatusZoneCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<APIResponse> Handle(ChangeStatusZoneCommand request, CancellationToken cancellationToken)
        {
            var zone = await _unitOfWork.ZoneRepository.GetById(request.Id);
            if (zone == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageCommon.NotFound
                };
            }
            zone.Status = request.Status;
            await _unitOfWork.ZoneRepository.Update(zone);

            if(await _unitOfWork.SaveChangesAsync())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.UpdateSuccesfully,
                    Data = zone.Id
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
