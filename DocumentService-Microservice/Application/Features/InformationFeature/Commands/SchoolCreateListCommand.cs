using System.Net;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.InformationModel;
using Application.Common.UUID;
using Application.Services.CacheService.Intefaces;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.InformationFeature.Commands;

public record SchoolCreateListCommand : IRequest<ResponseModel>
{
    public List<SchoolCreateRequestModel>? SchoolCreateRequestModel { get; init; }
}
public class SchoolCreateListCommandHandler(
    IUnitOfWork unitOfWork,
	ICleanCacheService cleanCacheService,
	IMapper mapper,
    ICurrentTime currentTime)
    : IRequestHandler<SchoolCreateListCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(SchoolCreateListCommand request, CancellationToken cancellationToken)
    {
        var school = mapper.Map<List<School>>(request.SchoolCreateRequestModel);
        await cleanCacheService.ClearRelatedCacheSchool();
		await unitOfWork.BeginTransactionAsync();
        foreach (var schoolData in school)
        {
            var provinceCheck = await unitOfWork.ProvinceRepository.GetByIdAsync(schoolData.ProvinceId, cancellationToken);
            if (provinceCheck is null)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy tỉnh thành này");
            }
            schoolData.Id = new UuidV7().Value;
            var result = await unitOfWork.SchoolRepository.CreateSchoolAsync(schoolData);
            if (result is false)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, $"Không thêm được trường {schoolData.SchoolName}");
            }
            
        }
        await unitOfWork.CommitTransactionAsync();
        return new ResponseModel(HttpStatusCode.OK, "Trường học đã được thêm");
    }
}