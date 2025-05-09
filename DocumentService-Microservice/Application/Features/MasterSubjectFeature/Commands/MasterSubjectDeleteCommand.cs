using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Constants;
using Application.Messages.SubjectMessage;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.MasterSubjectFeature.Commands;

public class MasterSubjectDeleteCommand : IRequest<ResponseModel>
{
    public Guid Id { get; init; }
}

public class MasterSubjectDeleteCommandHandler(IUnitOfWork unitOfWork, IProducerService producerService)
    : IRequestHandler<MasterSubjectDeleteCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(MasterSubjectDeleteCommand request, CancellationToken cancellationToken)
    {
		await unitOfWork.BeginTransactionAsync();
		var result = await unitOfWork.MasterSubjectRepository.DeleteMasterSubject(request.Id);
		var produceResult = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectDeleted, request.Id.ToString(), request.Id);
		if (produceResult is false)
		{
			await unitOfWork.RollbackTransactionAsync();
			return new ResponseModel(HttpStatusCode.BadRequest, "Tạo kafka thất bại");
		}
		if (result is true)
        {
			await unitOfWork.CommitTransactionAsync();
			return new ResponseModel(HttpStatusCode.Created,ResponseConstaints.MasterSubjectDeleted);
        }
		await unitOfWork.RollbackTransactionAsync();
		return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.MasterSubjectDeleteFailed);
    }
}