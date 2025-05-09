using System.Net;
using Application.Common.Models;
using Application.Common.Models.InformationModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.InformationFeature.Commands;

public record ProvinceCreateListCommand : IRequest<ResponseModel>
{
    public List<ProvinceCreateRequestModel> ProvinceCreateRequestModels { get; init; } = new();
}
public class ProvinceCreateListCommandHandler(
    IUnitOfWork unitOfWork,
	IMapper mapper)
    : IRequestHandler<ProvinceCreateListCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(ProvinceCreateListCommand request, CancellationToken cancellationToken)
    {
        // check if already have that id then update
        var provinces = mapper.Map<List<Province>>(request.ProvinceCreateRequestModels);
        await unitOfWork.BeginTransactionAsync();
        foreach (var province in provinces)
        {
            var result = await unitOfWork.ProvinceRepository.CreateProvinceAsync(province);
            if (result is false)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, $"Không thể thêm tỉnh thành {province.ProvinceName}");
            }
        }
        await unitOfWork.CommitTransactionAsync();
        return new ResponseModel(HttpStatusCode.Created, "Tất cả các tỉnh thành đã được thêm thành công");
    }
}