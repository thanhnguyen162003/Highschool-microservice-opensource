using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.MasterSubjectModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.Messages.SubjectMessage;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.MasterSubjectFeature.Commands;

public class MasterSubjectCreateCommand : IRequest<ResponseModel>
{
    public required MasterSubjectCreateRequestModel MasterSubjectCreateRequestModel { get; init; }
}

public class MasterSubjectCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IProducerService producerService)
    : IRequestHandler<MasterSubjectCreateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(MasterSubjectCreateCommand request, CancellationToken cancellationToken)
    {
        MasterSubject masterSubject = mapper.Map<MasterSubject>(request.MasterSubjectCreateRequestModel);
        masterSubject.Id = new UuidV7().Value;
        masterSubject.CreatedAt = DateTime.UtcNow;
        masterSubject.UpdatedAt = DateTime.UtcNow;
        masterSubject.MasterSubjectSlug = SlugHelper.GenerateSlug(masterSubject.MasterSubjectName);
		await unitOfWork.BeginTransactionAsync();
		SubjectMessage subjectMessage = new SubjectMessage()
		{
			SubjectId = masterSubject.Id,
			MasterSubjectName = masterSubject.MasterSubjectName,
			MasterSubjectSlug = masterSubject.MasterSubjectSlug
		};
		var result = await unitOfWork.MasterSubjectRepository.AddMasterSubject(masterSubject);
		var produceResult = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectCreated, masterSubject.Id.ToString(), subjectMessage);
		if (produceResult is false)
		{
			await unitOfWork.RollbackTransactionAsync();
			return new ResponseModel(HttpStatusCode.BadRequest, "Tạo kafka thất bại");
		}
		if (result is true)
        {
			await unitOfWork.CommitTransactionAsync();
			return new ResponseModel(HttpStatusCode.Created,ResponseConstaints.MasterSubjectCreated);
        }
		await unitOfWork.RollbackTransactionAsync();
		return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.MasterSubjectCreateFailed);
    }
}