using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Constants;
using Application.KafkaMessageModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.ChapterFeature.Commands;

public record DeleteChapterCommand : IRequest<ResponseModel>
{
    public Guid Id { get; init; }
}
public class DeleteChapterCommandHandler(IUnitOfWork unitOfWork, IProducerService producerService)
    : IRequestHandler<DeleteChapterCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(DeleteChapterCommand request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.ChapterRepository.DeleteChapter(request.Id);

        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.ChapterDeleteFailed);
        }
        //produce later
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.ChapterDeleted);
    }
}