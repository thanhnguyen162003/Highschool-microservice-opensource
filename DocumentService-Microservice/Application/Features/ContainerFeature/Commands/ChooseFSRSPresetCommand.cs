using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.ContainerModel;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.ContainerFeature.Commands
{
	public record UpdateContainerCommand : IRequest<ResponseModel>
	{
		public ContainerUpdateRequestModel? ContainerUpdateRequestModel;
		public Guid FlashcardId;
	}
	public class UpdateContainerCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper,
	IClaimInterface claimInterface)
	: IRequestHandler<UpdateContainerCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(UpdateContainerCommand request, CancellationToken cancellationToken)
		{
			var userId = claimInterface.GetCurrentUserId;
			var container = await unitOfWork.ContainerRepository.GetContainerByUserId(userId, request.FlashcardId, cancellationToken);
			if (container is null)
			{
				return new ResponseModel(HttpStatusCode.BadRequest, "Không thể tìm container cho user");
			}

			if (request.ContainerUpdateRequestModel is not null)
			{
				container.ShuffleFlashcards = request.ContainerUpdateRequestModel.ShuffleFlashcards ?? container.ShuffleFlashcards;
				container.LearnRound = request.ContainerUpdateRequestModel.LearnRound ?? container.LearnRound;
				container.LearnMode = request.ContainerUpdateRequestModel.LearnMode ?? container.LearnMode;
				container.ShuffleLearn = request.ContainerUpdateRequestModel.ShuffleLearn ?? container.ShuffleLearn;
				container.StudyStarred = request.ContainerUpdateRequestModel.StudyStarred ?? container.StudyStarred;
				container.AnswerWith = request.ContainerUpdateRequestModel.AnswerWith ?? container.AnswerWith;
				container.MultipleAnswerMode = request.ContainerUpdateRequestModel.MultipleAnswerMode ?? container.MultipleAnswerMode;
				container.ExtendedFeedbackBank = request.ContainerUpdateRequestModel.ExtendedFeedbackBank ?? container.ExtendedFeedbackBank;
				container.EnableCardsSorting = request.ContainerUpdateRequestModel.EnableCardsSorting ?? container.EnableCardsSorting;
				container.CardsRound = request.ContainerUpdateRequestModel.CardsRound ?? container.CardsRound;
				container.CardsStudyStarred = request.ContainerUpdateRequestModel.CardsStudyStarred ?? container.CardsStudyStarred;
				container.CardsAnswerWith = request.ContainerUpdateRequestModel.CardsAnswerWith ?? container.CardsAnswerWith;
				container.MatchStudyStarred = request.ContainerUpdateRequestModel.MatchStudyStarred ?? container.MatchStudyStarred;
				container.CardsPerDay = request.ContainerUpdateRequestModel.CardsPerDay ?? container.CardsPerDay;
			}

			var result = await unitOfWork.ContainerRepository.UpdateContainer(container, cancellationToken);
			if (!result)
			{
				return new ResponseModel(HttpStatusCode.BadRequest, "Không thể update container");
			}

			return new ResponseModel(HttpStatusCode.OK, "Cập nhật container thành công");
		}

	}
}