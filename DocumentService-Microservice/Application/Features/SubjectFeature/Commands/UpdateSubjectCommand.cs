using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Ultils;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectFeature.Commands;

public record UpdateSubjectCommand : IRequest<ResponseModel>
{
    public SubjectModel SubjectModel;
}
public class UpdateSubjectCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
    IProducerService producer, ICloudinaryService cloudinaryService,
    ILogger<UpdateSubjectCommandHandler> logger)
    : IRequestHandler<UpdateSubjectCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var subjectUpdateData = mapper.Map<Subject>(request.SubjectModel);
            if (request.SubjectModel.SubjectName is not null)
            {
                subjectUpdateData.Slug = SlugHelper.GenerateSlug(request.SubjectModel.SubjectName, request.SubjectModel.Id.ToString());
            }

            if (request.SubjectModel.CategoryId is not null)
            {
                var categoryCheck = await unitOfWork.CategoryRepository.GetByIdAsync(subjectUpdateData.CategoryId, cancellationToken);
                if (categoryCheck is null)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest,"Không tìm thấy danh mục");
                }
            }
            await unitOfWork.BeginTransactionAsync();
            var result = await unitOfWork.SubjectRepository.UpdateSubject(subjectUpdateData, cancellationToken);
            if (result is false)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectUpdateFailed);
            }
        
            if (request.SubjectModel.ImageRaw is not null)
            {
                var uploadResult = await cloudinaryService.UploadAsync(request.SubjectModel.ImageRaw);
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
                var produceResult = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectImageCreated, subjectUpdateData.Id.ToString(), model);
                if (produceResult is false)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadRequest, "Tạo kafka thất bại");
                }
            }
            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.SubjectUpdated, subjectUpdateData.Id);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackTransactionAsync();
            logger.LogError(e, e.Message);
            return new ResponseModel(HttpStatusCode.BadRequest, "Update Subject Fail In Catch");
        }
    }
}