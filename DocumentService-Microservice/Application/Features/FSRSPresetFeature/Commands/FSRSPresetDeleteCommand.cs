using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FSRSPresetModel;
using Application.Common.Models.InformationModel;
using Application.Common.UUID;
using Domain.Entities;
using Domain.Enums;
using Google.Apis.Util;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FSRSPresetFeature.Commands;

public record FSRSPresetDeleteCommand : IRequest<ResponseModel>
{
    public Guid Id { get; set; }
}
public class FSRSPresetDeleteCommandHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claimInterface)
    : IRequestHandler<FSRSPresetDeleteCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(FSRSPresetDeleteCommand request, CancellationToken cancellationToken)
    {
        var role = claimInterface.GetRole;
        var userId = claimInterface.GetCurrentUserId;
        var check = await unitOfWork.FSRSPresetRepository.GetPresetById(request.Id);
        if (check is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy preset");
        }
        if (role != RoleEnum.Admin.ToString() && role != RoleEnum.Moderator.ToString())
        {
            return new ResponseModel(HttpStatusCode.Forbidden, "Bạn không có quyền truy cập");
        }

        //if (check.UserId != userId)
        //{
        //    if (role != RoleEnum.Admin.ToString() && role != RoleEnum.Moderator.ToString())
        //    {
        //        return new ResponseModel(HttpStatusCode.Forbidden, "Preset này không phải của bạn");
        //    }
        //}

        var result = await unitOfWork.FSRSPresetRepository.DeletePreset(check.Id, cancellationToken);
        if (result is true)
        {
            return new ResponseModel(HttpStatusCode.Created, $"Preset {check.Id} đã được xóa");
        }
        return new ResponseModel(HttpStatusCode.BadRequest, $"Preset {check.Id} chưa được xóa");
    }
}