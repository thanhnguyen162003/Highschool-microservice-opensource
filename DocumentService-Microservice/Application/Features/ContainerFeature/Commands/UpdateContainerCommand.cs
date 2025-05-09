using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.ContainerModel;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.ContainerFeature.Commands
{
	public record ChooseFSRSPresetCommand : IRequest<ResponseModel>
	{
		public Guid FSRSPresetId;
        public Guid FlashcardId;
	}
	public class ChooseFSRSPresetCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper,
	IClaimInterface claimInterface)
	: IRequestHandler<ChooseFSRSPresetCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(ChooseFSRSPresetCommand request, CancellationToken cancellationToken)
		{
			var userId = claimInterface.GetCurrentUserId;
			var container = await unitOfWork.ContainerRepository.GetContainerByUserId(userId, request.FlashcardId, cancellationToken);
			if (container is null)
			{
				return new ResponseModel(HttpStatusCode.BadRequest, "Không thể tìm container cho user");
			}
            var preset= await unitOfWork.FSRSPresetRepository.GetPresetParameterAndR(request.FSRSPresetId);
            if (preset is null)
			{
                return new ResponseModel(HttpStatusCode.BadRequest, "Không thể tìm preset");
            }

            var (parameter, retrievability) = preset.Value;
            container.Retrievability = retrievability;
            container.FsrsParameters = parameter;

            var result = await unitOfWork.ContainerRepository.UpdateContainer(container, cancellationToken);
			if (!result)
			{
				return new ResponseModel(HttpStatusCode.BadRequest, "Không thể update container");
			}

			return new ResponseModel(HttpStatusCode.OK, "Cập nhật container thành công");
		}

	}
}