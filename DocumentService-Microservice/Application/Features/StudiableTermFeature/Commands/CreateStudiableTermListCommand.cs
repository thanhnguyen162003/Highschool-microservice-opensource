using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.StudiableTermModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.StudiableTermFeature.Commands;

public record CreateStudiableTermListCommand : IRequest<ResponseModel>
{
    public List<StudiableTermRequestModel>? StudiableTermRequestModels { get; init; }
}

public class CreateStudiableTermListCommandHandler(
    ILogger<CreateStudiableTermListCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claimInterface)
    : IRequestHandler<CreateStudiableTermListCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateStudiableTermListCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        if (request.StudiableTermRequestModels.IsNullOrEmpty())
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Bạn không học gì à???");
        }

        Guid currentUserId = claimInterface.GetCurrentUserId;
        if (currentUserId == Guid.Empty)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Người dùng chưa đăng nhập");
        }

        // Fetch FlashcardContent by IDs
        var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByIds(
            request.StudiableTermRequestModels.Select(x => x.FlashcardContentId).ToList());
        if (!flashcardContent.Any())
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy nội dung thẻ ghi nhớ nào");
        }

        // Check if all requested FlashcardContent IDs exist
        if (request.StudiableTermRequestModels.Count != flashcardContent.Count())
        {
            var missingIds = request.StudiableTermRequestModels
                .Select(x => x.FlashcardContentId)
                .Except(flashcardContent.Select(fc => fc.Id))
                .ToList();
            return new ResponseModel(HttpStatusCode.BadRequest, 
                "Không tìm thấy nội dung thẻ ghi nhớ: " + string.Join(", ", missingIds));
        }
        
        var sampleFlashcardContent = flashcardContent.First();
        Guid flashcardId = sampleFlashcardContent.FlashcardId;
        
        var container = await unitOfWork.ContainerRepository.GetContainerByUserId(currentUserId, flashcardId, cancellationToken);
        if (container == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy container");
        }

        var checkDup = await unitOfWork.StudiableTermRepository.CheckDuplicateStudiableTerm(
            currentUserId, 
            request.StudiableTermRequestModels.Select(x => x.FlashcardContentId).ToList(), 
            container.Id);
        
        if (checkDup.Count == request.StudiableTermRequestModels.Count)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Này là update chứ có phải create đâu ní");
        }

        foreach (var item in checkDup)
        {
            item.Mode = (bool)request.StudiableTermRequestModels
                .FirstOrDefault(x => x.FlashcardContentId.Equals(item.FlashcardContentId))?.Mode 
                ? "Learn" 
                : "Flashcard";
        }

        request.StudiableTermRequestModels.RemoveAll(x => checkDup.Select(d => d.FlashcardContentId).Contains(x.FlashcardContentId));

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.StudiableTermRepository.UpdateStudiableTermList(checkDup, cancellationToken);

        var studiableTerms = mapper.Map<List<StudiableTerm>>(request.StudiableTermRequestModels);
        foreach (var studiableTerm in studiableTerms)
        {
            studiableTerm.UserId = currentUserId;
            studiableTerm.ContainerId = container.Id;
        }

        var result = await unitOfWork.StudiableTermRepository.CreateStudiableTermList(studiableTerms, cancellationToken);
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

        studiableTerms.AddRange(checkDup);

        foreach (var studiableTerm in studiableTerms)
        {
            studiableTerm.FlashcardContent = flashcardContent
                .FirstOrDefault(x => x.Id.Equals(studiableTerm.FlashcardContentId));
        }

        var response = mapper.Map<List<StudiableTermResponseModel>>(studiableTerms);
        foreach (var item in response)
        {
            if (sampleFlashcardContent.StarredTerm.Count > 0)
            {
                item.FlashcardContent.IsStarred = sampleFlashcardContent.StarredTerm.Any(x => x.UserId == claimInterface.GetCurrentUserId);
            }
            if (sampleFlashcardContent.StudiableTerm.Count > 0)
            {
                item.FlashcardContent.IsLearned = sampleFlashcardContent.StudiableTerm.Any(x => x.UserId == claimInterface.GetCurrentUserId);
            }
        }
        return new ResponseModel
        {
            Status = HttpStatusCode.OK,
            Message = "Success",
            Data = response
        };
    }
}
