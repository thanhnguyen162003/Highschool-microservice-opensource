using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SubjectModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.Entities;
using Domain.Events;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectFeature.Commands;

public record CreateSubjectCommand : IRequest<ResponseModel>
{
    public SubjectCreateRequestModel SubjectCreateModel;
}
public class CreateSubjectCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IProducerService producer,
    ICloudinaryService cloudinary,
    ILogger<CreateSubjectCommandHandler> logger)
    : IRequestHandler<CreateSubjectCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        try {
            var subjectUpdateData = mapper.Map<Subject>(request.SubjectCreateModel);
            subjectUpdateData.Id = new UuidV7().Value;
            subjectUpdateData.SubjectCode = request.SubjectCreateModel.SubjectCode.ToUpper();
            subjectUpdateData.Slug = SlugHelper.GenerateSlug(subjectUpdateData.SubjectName, subjectUpdateData.Id.ToString());
            subjectUpdateData.AddDomainEvent(new CreateSubjectEvent(subjectUpdateData));
            var categoryCheck = await unitOfWork.CategoryRepository.GetByIdAsync(subjectUpdateData.CategoryId, cancellationToken);
            if (categoryCheck is null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest,"Không tìm thấy danh mục");
            }
            
            await unitOfWork.BeginTransactionAsync();
            var result = await unitOfWork.SubjectRepository.CreateSubject(subjectUpdateData, cancellationToken);
            if (result is false)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectCreateFailed);
            }
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
                await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectImageCreated, subjectUpdateData.Id.ToString(), model);
            }
            var produceResult = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectCreated, subjectUpdateData.Id.ToString(), subjectUpdateData.Id);
            if (produceResult is false)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, "Tạo kafka thất bại");
            }
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