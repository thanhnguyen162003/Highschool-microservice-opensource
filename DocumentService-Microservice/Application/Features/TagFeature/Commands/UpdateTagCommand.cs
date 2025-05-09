using Application.Common.Models;
using Application.Common.Models.TagModel;
using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.TagFeature.Commands;

public record UpdateTagCommand : IRequest<ResponseModel>
{
    public Guid TagId { get; set; }
    public TagUpdateRequestModel UpdateTagRequest { get; set; } = null!;
}

public class UpdateTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateTagCommand, ResponseModel>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseModel> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UpdateTagRequest.Name))
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Tên tag không được để trống");
        }

        var tag = await _unitOfWork.TagRepository.GetByIdAsync(request.TagId, cancellationToken);
        if (tag == null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Tag không tồn tại");
        }

        var tagName = request.UpdateTagRequest.Name.Trim();
        var normalizedName = StringHelper.NormalizeVietnamese(tagName);

        if (!string.Equals(tag.NormalizedName, normalizedName, StringComparison.OrdinalIgnoreCase))
        {
            var existingTags = await _unitOfWork.TagRepository.SearchTagsAsync(
                string.Empty, normalizedName, 1, cancellationToken);

            if (existingTags.Any(t => string.Equals(t.NormalizedName, normalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                return new ResponseModel(HttpStatusCode.Conflict, "Tag này đã tồn tại");
            }
        }

        tag.Name = tagName;
        tag.NormalizedName = normalizedName;
        tag.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.TagRepository.Update(tag);
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<TagResponseModel>(tag);
        return new ResponseModel(HttpStatusCode.OK, "Cập nhật tag thành công.", response);
    }
} 