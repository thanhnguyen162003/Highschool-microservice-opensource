using System.Net;
using Algolia.Search.Models.Search;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.StudiableTermModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.StudiableTermFeature.Commands;

public record CreateStudiableTermCommand : IRequest<ResponseModel>
{
    public StudiableTermRequestModel StudiableTermRequestModels { get; init; }
}

public class CreateStudiableTermCommandHandler(
    ILogger<CreateStudiableTermCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claimInterface)
    : IRequestHandler<CreateStudiableTermCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateStudiableTermCommand request, CancellationToken cancellationToken)
    {
        Guid currentUserId = claimInterface.GetCurrentUserId;
        if (currentUserId == Guid.Empty)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Người dùng chưa đăng nhập");
        }

        // Fetch FlashcardContent by IDs
        var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentById(request.StudiableTermRequestModels.FlashcardContentId);
        if (flashcardContent is null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy nội dung thẻ ghi nhớ");
        }

        Guid flashcardId = flashcardContent.FlashcardId;
        
        var container = await unitOfWork.ContainerRepository.GetContainerByUserId(currentUserId, flashcardId, cancellationToken);
        if (container == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy container");
        }
        List<Guid> listFlashcardContentIds = new List<Guid>();
        listFlashcardContentIds.Add(request.StudiableTermRequestModels.FlashcardContentId);
        var checkDup = await unitOfWork.StudiableTermRepository.CheckDuplicateStudiableTerm(
            currentUserId,
            listFlashcardContentIds, 
            container.Id);
        
        if (checkDup.Count == listFlashcardContentIds.Count)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Này là update chứ có phải create đâu ní");
        }

        var studiableTerm = mapper.Map<StudiableTerm>(request.StudiableTermRequestModels);
       
        studiableTerm.UserId = currentUserId;
        studiableTerm.ContainerId = container.Id;
        
        await unitOfWork.BeginTransactionAsync();
        var result = await unitOfWork.StudiableTermRepository.CreateStudiableTerm(studiableTerm, cancellationToken);
        if (!result)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Internal Server Error"
            };
        }
        
        await unitOfWork.CommitTransactionAsync();

        studiableTerm.FlashcardContent = flashcardContent;
        
        var response = mapper.Map<StudiableTermResponseModel>(studiableTerm);
        

        if (flashcardContent.StarredTerm.Count > 0)
        {
            response.FlashcardContent.IsStarred = flashcardContent.StarredTerm.Any(x => x.UserId == currentUserId);
        }
        if (flashcardContent.StudiableTerm.Count > 0)
        {
            response.FlashcardContent.IsLearned = flashcardContent.StudiableTerm.Any(x => x.UserId == currentUserId);
        }
            
        

        return new ResponseModel
        {
            Status = HttpStatusCode.OK,
            Message = "Success",
            Data = response
        };
    }
}
