using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.StudiableTermModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.StudiableTermFeature.Commands;

public record UpdateStudiableTermListCommand : IRequest<ResponseModel>
{
    public List<StudiableTermRequestModel>? StudiableTermRequestModels { get; init; }
}

public class UpdateStudiableTermListCommandHandler(
    ILogger<UpdateStudiableTermListCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claimInterface)
    : IRequestHandler<UpdateStudiableTermListCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateStudiableTermListCommand request, CancellationToken cancellationToken)
    {
        if (request.StudiableTermRequestModels.IsNullOrEmpty())
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Bạn không học gì à???");
        }

        if (claimInterface.GetCurrentUserId == Guid.Empty)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Người dùng chưa đăng nhập");
        }

        var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByIds(request.StudiableTermRequestModels.Select(x => x.FlashcardContentId).ToList());
        if (flashcardContent.Count() == 0)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy nội dung thẻ ghi nhớ nào ");
        }
        
        var sampleFlashcardContent = flashcardContent.First();
        Guid flashcardId = sampleFlashcardContent.FlashcardId;

        if (request.StudiableTermRequestModels.Count != flashcardContent.Count())
        {
            List<Guid> flashcardContentIds = new List<Guid>();
            for (int i = 0; i < request.StudiableTermRequestModels.Count; i++)
            {
                for (int j = 0; j < flashcardContent.Count(); j++)
                {
                    if (request.StudiableTermRequestModels[i].FlashcardContentId != flashcardContent[j].Id)
                    {
                        flashcardContentIds.Add(request.StudiableTermRequestModels[i].FlashcardContentId);
                    }
                }
            }
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy nội dung thẻ ghi nhớ " + string.Join(", ", flashcardContentIds));

        }

        var container = await unitOfWork.ContainerRepository.GetContainerByUserId(claimInterface.GetCurrentUserId, flashcardId, cancellationToken);
        if (container == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy container ");
        }

        var checkDup = await unitOfWork.StudiableTermRepository.CheckDuplicateStudiableTerm(claimInterface.GetCurrentUserId, request.StudiableTermRequestModels.Select(x => x.FlashcardContentId).ToList(), container.Id);
        foreach (var item in checkDup)
        {
            item.Mode = request.StudiableTermRequestModels.FirstOrDefault(x => x.FlashcardContentId.Equals(item.FlashcardContentId)).Mode ? "Learn" : "Flashcard"; ;
        }
        request.StudiableTermRequestModels.RemoveAll(x => checkDup.Select(x => x.FlashcardContentId).Contains(x.FlashcardContentId));

        await unitOfWork.BeginTransactionAsync();
        var createStudiableTerms = mapper.Map<List<StudiableTerm>>(request.StudiableTermRequestModels);
        if (request.StudiableTermRequestModels.Count > 0)
        {
            
            foreach (var studiableTerm in createStudiableTerms)
            {
                studiableTerm.UserId = claimInterface.GetCurrentUserId;
                studiableTerm.ContainerId = container.Id;
            }

            var create = await unitOfWork.StudiableTermRepository.CreateStudiableTermList(createStudiableTerms, cancellationToken);
            if (create is false)
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Internal Server Error"
                };
            }
        }


        var result = await unitOfWork.StudiableTermRepository.UpdateStudiableTermList(checkDup, cancellationToken);
        if (result is false)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Internal Server Error"
            };
        }
        await unitOfWork.CommitTransactionAsync();
        checkDup.AddRange(createStudiableTerms);
        for (int i = 0; i < checkDup.Count; i++)
        {
            checkDup[i].FlashcardContent = flashcardContent.FirstOrDefault(x => x.Id.Equals(checkDup[i].FlashcardContentId));
        }
        var response = mapper.Map<List<StudiableTermResponseModel>>(checkDup);

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