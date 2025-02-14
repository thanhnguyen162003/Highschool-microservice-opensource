using Application.Messages;
using Domain.Models.Common;
using Infrastructure;
using MediatR;
using System.Net;

namespace Application.Features.UserFeatures.Command
{
    public class UpdateBackgroundAvatarCommand : IRequest<APIResponse>
    {
        public string Image { get; set; } = null!;
        public string Type { get; set; } = null!;
    }

    public class UpdateBackgroundAvatarCommandHandler : IRequestHandler<UpdateBackgroundAvatarCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBackgroundAvatarCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<APIResponse> Handle(UpdateBackgroundAvatarCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.AvatarRepository.UpdateBackground(request.Image, request.Type);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.UpdateSuccesfully
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Message = MessageCommon.UpdateFailed
            };
        }
    }

}
