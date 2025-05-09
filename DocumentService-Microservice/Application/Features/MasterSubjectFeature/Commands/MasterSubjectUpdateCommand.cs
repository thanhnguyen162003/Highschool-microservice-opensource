using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.MasterSubjectModel;
using Application.Common.Ultils;
using Application.Constants;
using Application.Messages.SubjectMessage;
using Application.Services;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.MasterSubjectFeature.Commands;

public class MasterSubjectUpdateCommand : IRequest<ResponseModel>
{
    public required MasterSubjectCreateRequestModel MasterSubject { get; init; }
    public Guid Id { get; init; }
}

public class MasterSubjectUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IProducerService producerService)
    : IRequestHandler<MasterSubjectUpdateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(MasterSubjectUpdateCommand request, CancellationToken cancellationToken)
    {
        MasterSubject masterSubject = mapper.Map<MasterSubject>(request.MasterSubject);
        masterSubject.Id = request.Id;
        masterSubject.UpdatedAt = DateTime.UtcNow;
        masterSubject.MasterSubjectSlug = SlugHelper.GenerateSlug(request.MasterSubject.MasterSubjectName);
		await unitOfWork.BeginTransactionAsync();
		var result = await unitOfWork.MasterSubjectRepository.UpdateMasterSubject(masterSubject);
		SubjectMessage subjectMessage = new SubjectMessage()
		{
			SubjectId = masterSubject.Id,
			MasterSubjectName = masterSubject.MasterSubjectName,
			MasterSubjectSlug = masterSubject.MasterSubjectSlug
		};
		var produceResult = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectCreated, masterSubject.Id.ToString(), subjectMessage);
		if (produceResult is false)
		{
			await unitOfWork.RollbackTransactionAsync();
			return new ResponseModel(HttpStatusCode.BadRequest, "Tạo kafka thất bại");
		}
		if (result is true)
        {
			await unitOfWork.CommitTransactionAsync();
			return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.MasterSubjectUpdated);
        }
		await unitOfWork.RollbackTransactionAsync();
		return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.MasterSubjectUpdateFailed);
    }
}