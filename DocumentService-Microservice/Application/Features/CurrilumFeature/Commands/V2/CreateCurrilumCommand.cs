//using System.Net;
//using Application.Common.Models;
//using Application.Common.Models.CurriculumModel;
//using Application.Common.UUID;
//using Application.Constants;
//using Domain.Entities;
//using Infrastructure.Repositories.Interfaces;

//namespace Application.Features.CurrilumFeature.Commands.V2;

//public record CreateCurrilumCommand : IRequest<ResponseModel>
//{
//    public required CurriculumCreateRequestModel CurriculumCreateRequestModel;
//}

//public class CreateCurrilumCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//    : IRequestHandler<CreateCurrilumCommand, ResponseModel>
//{
//    public async Task<ResponseModel> Handle(CreateCurrilumCommand request, CancellationToken cancellationToken)
//    {
//        var listSubject = await unitOfWork.SubjectRepository.GetAllSubjects(cancellationToken);
//		var curriculum = mapper.Map<Curriculum>(request.CurriculumCreateRequestModel);
//        curriculum.Id = new UuidV7().Value;
//        curriculum.CreatedAt = DateTime.UtcNow;
//        curriculum.UpdatedAt = DateTime.UtcNow;
//        await unitOfWork.BeginTransactionAsync();
//		await unitOfWork.CurriculumRepository.InsertAsync(curriculum);
//		var result = await unitOfWork.SaveChangesAsync();
//		if (result <= 0)
//		{
//			await unitOfWork.RollbackTransactionAsync();
//			return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.CurriculumCreateFailed);
//		}
//		foreach (var subject in listSubject)
//		{
//			var subjectCurriculum = new SubjectCurriculum
//			{
//				Id = new UuidV7().Value,
//				CurriculumId = curriculum.Id,
//				SubjectId = subject.Id,
//				IsPublish = false,
//				SubjectCurriculumName = subject.SubjectName + " " + curriculum.CurriculumName,
//			};
//			curriculum.SubjectCurricula.Add(subjectCurriculum);
//		}
//		return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.CurriculumCreated);
//    }
//}