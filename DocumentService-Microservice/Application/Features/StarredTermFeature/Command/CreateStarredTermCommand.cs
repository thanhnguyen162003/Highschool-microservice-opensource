using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.StarredTermFeature.Commands
{
    public record CreateStarredTermCommand : IRequest<ResponseModel>
    {
        public Guid FlashcardContentId;
    }

    public class CreateStarredTermCommandHandler(IUnitOfWork unitOfWork,
    IClaimInterface claimInterface)
    : IRequestHandler<CreateStarredTermCommand, ResponseModel>
    {

        public async Task<ResponseModel> Handle(CreateStarredTermCommand request, CancellationToken cancellationToken)
        {
            if (claimInterface.GetCurrentUserId == Guid.Empty)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Người dùng chưa đăng nhập");
            }

            var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentById(request.FlashcardContentId);
            if (flashcardContent == null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy nội dung thẻ ghi nhớ ");
            }

            var container =
                await unitOfWork.ContainerRepository.GetContainerByUserId(claimInterface.GetCurrentUserId,
                    flashcardContent.FlashcardId, cancellationToken);
            if (container == null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy container ");
            }

            var checkDup = await unitOfWork.StarredTermRepository.CheckDuplicateStarredTerm(claimInterface.GetCurrentUserId, request.FlashcardContentId, container.Id);
            var starredTerm = new StarredTerm
            {
                UserId = claimInterface.GetCurrentUserId,
                FlashcardContentId = request.FlashcardContentId,
                ContainerId = container.Id,
            };
            if (checkDup)
            {
                try
                {
                    await unitOfWork.StarredTermRepository.DeleteStarredTerm(starredTerm);

                    return new ResponseModel
                    {
                        Status = System.Net.HttpStatusCode.OK,
                        Message = ResponseConstaints.StarDeleteSuccess
                        //Data = request.QuestionIds
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseModel
                    {
                        Status = System.Net.HttpStatusCode.InternalServerError,
                        Message = ResponseConstaints.StarDeleteFailed,
                        Data = ex.Message
                    };
                }
            }
            else
            {
                try
                {
                    await unitOfWork.StarredTermRepository.AddStarredTerm(starredTerm);

                    return new ResponseModel
                    {
                        Status = System.Net.HttpStatusCode.OK,
                        Message = ResponseConstaints.StarAddSuccess
                        //Data = request.QuestionIds
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseModel
                    {
                        Status = System.Net.HttpStatusCode.InternalServerError,
                        Message = ResponseConstaints.StarAddFailed,
                        Data = ex.Message
                    };
                }
            }
        }
            
    }


}
