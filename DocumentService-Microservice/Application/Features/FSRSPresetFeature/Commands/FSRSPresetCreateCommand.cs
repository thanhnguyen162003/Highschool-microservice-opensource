using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FSRSPresetModel;
using Application.Common.Models.InformationModel;
using Application.Common.UUID;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FSRSPresetFeature.Commands;

public record FSRSPresetCreateCommand : IRequest<ResponseModel>
{
    public FSRSPresetCreateRequest fSRSPresetCreate { get; init; }
}
public class FSRSPresetCreateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper, IClaimInterface claimInterface)
    : IRequestHandler<FSRSPresetCreateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(FSRSPresetCreateCommand request, CancellationToken cancellationToken)
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
        
        var preset = mapper.Map<FSRSPreset>(request.fSRSPresetCreate);
        preset.UserId = userId;
        preset.Id = new UuidV7().Value;
        preset.CreatedAt = DateTime.UtcNow;
        preset.UpdatedAt = DateTime.UtcNow;
        var result = await unitOfWork.FSRSPresetRepository.CreatePreset(preset);
        if (result is true)
        {
            return new ResponseModel(HttpStatusCode.Created, $"Preset {preset.Id} đã được thêm");
        }
        return new ResponseModel(HttpStatusCode.BadRequest, $"Preset {preset.Id} chưa được thêm");
    }
}