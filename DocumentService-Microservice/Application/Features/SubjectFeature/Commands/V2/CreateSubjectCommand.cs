using Application.Common.Interfaces;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Common.Models.SubjectModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.SubjectFeature.Commands.V2;

public record CreateSubjectCommand : IRequest<ResponseModel>
{
    public required SubjectCreateRequestModel SubjectCreateModel;
}
public class CreateSubjectCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICloudinaryService cloudinary,
	IProducerService producer,
    ILogger<CreateSubjectCommandHandler> logger)
    : IRequestHandler<CreateSubjectCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        try {
            var subjectUpdateData = mapper.Map<Subject>(request.SubjectCreateModel);
            subjectUpdateData.Id = new UuidV7().Value;
            subjectUpdateData.SubjectCode = request.SubjectCreateModel.SubjectCode?.ToUpper();
            subjectUpdateData.Slug = SlugHelper.GenerateSlug(subjectUpdateData.SubjectName, subjectUpdateData.Id.ToString());
            subjectUpdateData.AddDomainEvent(new CreateSubjectEvent(subjectUpdateData));
            if (subjectUpdateData.MasterSubjectId != null)
            {
                var masterSubjectCheck = await unitOfWork.MasterSubjectRepository.GetByIdAsync(subjectUpdateData.MasterSubjectId, cancellationToken);
                if (masterSubjectCheck is null)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest,"Không tìm thấy danh mục");
                }
            }
            await unitOfWork.BeginTransactionAsync();

			if (request.SubjectCreateModel.ImageRaw is not null)
			{
				var uploadResult = await cloudinary.UploadAsync(request.SubjectCreateModel.ImageRaw);
				if (uploadResult.Error != null)
				{
					return new ResponseModel(HttpStatusCode.BadRequest, uploadResult.Error.Message);
				}
				SubjectImageCreatedModel model = new SubjectImageCreatedModel()
				{
					ImageUrl = uploadResult.Url.ToString(),
					SubjectId = subjectUpdateData.Id,
					PublicIdUrl = uploadResult.PublicId,
					Format = uploadResult.Format
				};
				subjectUpdateData.Image = model.ImageUrl;
			}
			var result = await unitOfWork.SubjectRepository.CreateSubject(subjectUpdateData, cancellationToken);
			if (result is false)
			{
				await unitOfWork.RollbackTransactionAsync();
				return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectCreateFailed);
			}
			if (request.SubjectCreateModel.IsExternal == false)
			{
				var curricula = await unitOfWork.CurriculumRepository.GetAllNotExternal();
				foreach (var curriculum in curricula)
				{
					var subjectCurriculum = new SubjectCurriculum()
					{
						Id = new UuidV7().Value,
						CurriculumId = curriculum.Id,
						SubjectId = subjectUpdateData.Id,
						IsPublish = false,
						SubjectCurriculumName = subjectUpdateData.SubjectName + " " + curriculum.CurriculumName
					};
					var result1 = await unitOfWork.SubjectCurriculumRepository.AddSubjectCurriculum(subjectCurriculum, cancellationToken);
					if (result1 is false)
					{
						await unitOfWork.RollbackTransactionAsync();
						return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectCurriculumCreateFailed);
					}
				}
			}
            var produceResultCourse = await producer.ProduceObjectWithKeyAsync(
                TopicKafkaConstaints.DataSearchModified,
                subjectUpdateData.Id.ToString(),
                new SearchEventDataModifiedModel
                {
                    Type = TypeEvent.Create,
                    IndexName = IndexName.course,
                    Data = new List<SearchEventDataModel>
                    {
                        new SearchEventDataModel
                        {
                            Id = subjectUpdateData.Id,
                            Name = subjectUpdateData.SubjectName,
                            TypeField = UpdateCourseType.subjectId
                        }
                    }
                });
            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.OK,ResponseConstaints.SubjectCreated, subjectUpdateData.Id);
        }catch (Exception e)
        {
            await unitOfWork.RollbackTransactionAsync();
            logger.LogError(e, ResponseConstaints.SubjectCreateFailed);
            return new ResponseModel(HttpStatusCode.BadRequest, "Fail in Catch Exception");
        }
    }
}