using System.Net;
using Application.Common.Models;
using Application.Common.Models.ChapterModel;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.ChapterFeature.Commands;

public record UpdateChapterCommand : IRequest<ResponseModel>
{
    public ChapterUpdateRequestModel ChapterUpdateRequestModel;
}
public class UpdateChapterCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<UpdateChapterCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateChapterCommand request, CancellationToken cancellationToken)
    {
        var chapterUpdateData = mapper.Map<Chapter>(request.ChapterUpdateRequestModel);
        var result = await unitOfWork.ChapterRepository.UpdateChapter(chapterUpdateData);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.ChapterUpdateFailed);
        }
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.ChapterUpdated, chapterUpdateData.Id);
    }
}