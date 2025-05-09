using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FSRSPresetModel;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FSRSPresetFeature.Commands;

public record FSRSPresetUpdateCommand : IRequest<ResponseModel>
{
    public Guid Id { get; set; }
    public FSRSPresetCreateRequest fSRSPresetCreate { get; init; }
}
public class FSRSPresetUpdateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claimInterface)
    : IRequestHandler<FSRSPresetUpdateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(FSRSPresetUpdateCommand request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        var role = claimInterface.GetRole;

        if (role != RoleEnum.Admin.ToString() && role != RoleEnum.Moderator.ToString())
        {
            return new ResponseModel(HttpStatusCode.Forbidden, "Bạn không có quyền truy cập");
        }
        //if (request.fSRSPresetCreate.IsPublicPreset == true)
        //{
        //    if (role != RoleEnum.Admin.ToString() && role != RoleEnum.Moderator.ToString() )
        //    {
        //        return new ResponseModel(HttpStatusCode.Forbidden, "Bạn không thể public preset của bạn");
        //    }
        //}

        var check = await unitOfWork.FSRSPresetRepository.GetPresetById(request.Id);
        if (check is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy preset");
        }

        if (check.UserId != userId )
        {
            if (role != RoleEnum.Admin.ToString() && role != RoleEnum.Moderator.ToString())
            {
                return new ResponseModel(HttpStatusCode.Forbidden, "Preset này không phải của bạn");
            }
        }

        mapper.Map(request.fSRSPresetCreate, check);
        check.UpdatedAt = DateTime.UtcNow;
        var result = await unitOfWork.FSRSPresetRepository.UpdatePreset(check, cancellationToken);

        if (result)
        {
            return new ResponseModel(HttpStatusCode.OK, $"Preset {check.Id} đã được cập nhật");
        }

        return new ResponseModel(HttpStatusCode.BadRequest, $"Preset {check.Id} chưa được cập nhật");
    }
}