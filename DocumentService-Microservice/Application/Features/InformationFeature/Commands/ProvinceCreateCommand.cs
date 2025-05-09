using System.Net;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.InformationModel;
using Application.Common.UUID;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.InformationFeature.Commands;

public record ProvinceCreateCommand : IRequest<ResponseModel>
{
    public ProvinceCreateRequestModel ProvinceCreateRequestModel { get; init; }
}
public class DoetCreateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentTime currentTime)
    : IRequestHandler<ProvinceCreateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(ProvinceCreateCommand request, CancellationToken cancellationToken)
    {
        var province = mapper.Map<Province>(request.ProvinceCreateRequestModel);
        var result = await unitOfWork.ProvinceRepository.CreateProvinceAsync(province);
        if (result is true)
        {
            return new ResponseModel(HttpStatusCode.Created, $"Tỉnh thành {province.ProvinceName} đã được thêm");
        }
        return new ResponseModel(HttpStatusCode.BadRequest, $"Tỉnh thành {province.ProvinceName} chưa được thêm");
    }
}