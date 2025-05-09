using Application.Common.Models;
using Application.Common.Models.TagModel;
using Application.Common.UUID;
using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repositories.Interfaces;
using System.Net;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Application.Features.TagFeature.Commands;

public record CreateTagCommand : IRequest<ResponseModel>
{
    public TagCreateRequestModel CreateTagRequest { get; set; } = null!;
}

public class CreateTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateTagCommand, ResponseModel>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseModel> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var resultList = new List<TagResponseModel>();

        foreach (var name in request.CreateTagRequest.Name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Tên tag không được để trống");
            }

            var tagName = name.Trim();
            var normalizedName = StringHelper.NormalizeVietnamese(tagName);

            var existingTags = await _unitOfWork.TagRepository.SearchTagsAsync(
                string.Empty, normalizedName, 1, cancellationToken);

            if (existingTags.Any(t => string.Equals(t.NormalizedName, normalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                return new ResponseModel(HttpStatusCode.Conflict, $"Tag {name} đã tồn tại");
            }

            var tag = new Tag
            {
                Id = new UuidV7().Value,
                Name = tagName,
                NormalizedName = normalizedName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TagRepository.InsertAsync(tag);            

            var response = _mapper.Map<TagResponseModel>(tag);

            resultList.Add(response);
        }
        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel(HttpStatusCode.OK, "Tạo tag thành công.", resultList);
    }
} 