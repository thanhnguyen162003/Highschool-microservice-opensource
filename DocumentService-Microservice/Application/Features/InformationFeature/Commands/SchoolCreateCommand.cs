using System.Net;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.InformationModel;
using Application.Common.UUID;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.InformationFeature.Commands;

public record SchoolCreateCommand : IRequest<ResponseModel>
{
    public SchoolCreateRequestModel? SchoolCreateRequestModel { get; init; }
    public int ProvinceId { get; init; }
}
public class SchoolCreateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentTime currentTime)
    : IRequestHandler<SchoolCreateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(SchoolCreateCommand request, CancellationToken cancellationToken)
    {
        var school = mapper.Map<School>(request.SchoolCreateRequestModel);
        school.Id = new UuidV7().Value;
        var provinceCheck = await unitOfWork.ProvinceRepository.GetByIdAsync(request.ProvinceId, cancellationToken);
        if (provinceCheck is null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy tỉnh thành này");
        }
        var result = await unitOfWork.SchoolRepository.CreateSchoolAsync(school);
        if (result is true)
        {
            return new ResponseModel(HttpStatusCode.Created, "Trường học đã được thêm");
        }
        return new ResponseModel(HttpStatusCode.BadRequest, "Không thêm được trường");
    }
}