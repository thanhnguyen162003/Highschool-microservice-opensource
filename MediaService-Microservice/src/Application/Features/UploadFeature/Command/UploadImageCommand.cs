using System.Net;
using Application.Common.Extentions;
using Application.Common.Interfaces.CloudinaryInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Models.ImageModels;
using Application.Common.Ultils;
using CloudinaryDotNet.Actions;
using static Domain.Enums.ImageOption;

namespace Application.Features.UploadFeature.Command;

public class UploadImageCommand : IRequest<ResponseModel>
{
    public IFormFile Image { get; set; } = null!;
    public ImageFolder Folder { get; set; }
    public PresetImage? Preset { get; set; }
    public string? FileName { get; set; }
}

public class UploadImageCommandHandler(ICloudinaryService cloudinaryService) : IRequestHandler<UploadImageCommand, ResponseModel>
{
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<ResponseModel> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        UploadResult? result = null;
        if (!request.Preset!.IsNull())
        {
            PresetImage preset = request.Preset!.GetPreset();
            var file = FileHelper.ResizeImage(preset, request.Image);
            var extension = Path.GetExtension(request.Image.FileName).TrimStart('.').ToLower();
            result = await _cloudinaryService.UploadImage(file, request.Folder, request.Preset.Format ?? extension.GetEnum<ImageFormat>(), request.FileName);
        } else
        {
            var extension = Path.GetExtension(request.Image.FileName).TrimStart('.').ToLower();
            result = await _cloudinaryService.UploadImage(request.Image, request.Folder, request.Preset?.Format ?? extension.GetEnum<ImageFormat>(), request.FileName);
        }

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return new ResponseModel
            {
                Status = result.StatusCode,
                Message = result.Error.Message
            };
        }

        return new ResponseModel
        {
            Status = result.StatusCode,
            Message = "Tải lên thành công.",
            Data = result.SecureUrl.ToString()
        };

    }
}
