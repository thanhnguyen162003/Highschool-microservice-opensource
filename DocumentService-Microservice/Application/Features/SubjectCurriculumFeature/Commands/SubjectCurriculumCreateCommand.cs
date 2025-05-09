using System.Net;
using Application.Common.Models;
using Application.Common.Models.SubjectCurriculumModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectCurriculumFeature.Commands;


public class SubjectCurriculumCreateCommand : IRequest<ResponseModel>
{
    public SubjectCurriculumCreateRequestModel SubjectCurriculumCreateRequestModel;
}
public class SubjectCurriculumCreateCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<SubjectCurriculumCreateCommandHandler> logger)
    : IRequestHandler<SubjectCurriculumCreateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(SubjectCurriculumCreateCommand request, CancellationToken cancellationToken)
{
    try
    {
        // Check if Subject exists
        var subjectCheck = await unitOfWork.SubjectRepository.SubjectIdExistAsync(request.SubjectCurriculumCreateRequestModel.SubjectId);
        if (!subjectCheck)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy môn học");
        }

        // Check if Curriculum exists
        var curriculumCheck = await unitOfWork.CurriculumRepository.GetByIdAsync(request.SubjectCurriculumCreateRequestModel.CurriculumId, cancellationToken);
        if (curriculumCheck is null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy chương trình học");
        }
        //check if subject curriculum is exits
        var subjectCurriculumCheck = await unitOfWork.SubjectCurriculumRepository
            .IsSubjectCurriculumExists(request.SubjectCurriculumCreateRequestModel.SubjectId, request.SubjectCurriculumCreateRequestModel.CurriculumId, cancellationToken);
        if (subjectCurriculumCheck is true)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Chương trình giảng dạy môn học này đã có rồi");
        }
        var subjectCurriculum = new SubjectCurriculum
        {
            Id = new UuidV7().Value,
            SubjectId = request.SubjectCurriculumCreateRequestModel.SubjectId,
            CurriculumId = request.SubjectCurriculumCreateRequestModel.CurriculumId,
            SubjectCurriculumName = request.SubjectCurriculumCreateRequestModel.SubjectCurriculumName,
            IsPublish = false
        };

        var curriculumResult = await unitOfWork.SubjectCurriculumRepository.AddSubjectCurriculum(subjectCurriculum, cancellationToken);
        if (!curriculumResult)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectCurriculumCreateFailed);
        }

        return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.SubjectCurriculumCreated, new
        {
            SubjectCurriculumId = subjectCurriculum.Id,
            SubjectId = subjectCurriculum.SubjectId,
            CurriculumId = subjectCurriculum.CurriculumId
        });
    }
    catch (Exception e)
    {
        await unitOfWork.RollbackTransactionAsync();
        logger.LogError(e, ResponseConstaints.SubjectCurriculumCreateFailed);
        return new ResponseModel(HttpStatusCode.BadRequest, "Fail in Catch Exception");
    }
}
}