using Application.Common.Models.External;
using Application.Common.Models.Settings;
using Application.Service.Cloudinary;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Settings;
using Microsoft.Extensions.Options;

namespace Application.Services.Cloudinary
{
	public class CloudinaryService : ICloudinaryService
	{
		private readonly CloudinaryDotNet.Cloudinary _cloudinary;
		private readonly DefaultSystem _defaultSystem;

		public CloudinaryService(IOptions<CloudinarySetting> config, IOptions<DefaultSystem> options)
		{
			_defaultSystem = options.Value;

			var account = new Account(
				config.Value.CloudName,
				config.Value.ApiKey,
				config.Value.ApiSecret);

			_cloudinary = new CloudinaryDotNet.Cloudinary(account);
		}

		public async Task<ImageResponse> UploadAsync(IFormFile file, string nameImage)
		{
			await using var stream = file.OpenReadStream();
			var uploadParams = new ImageUploadParams
			{
				File = new FileDescription(nameImage, stream),
				Folder = _defaultSystem.Abbreviation + "/" + "reports",
				PublicId = nameImage
			};

			var result = await _cloudinary.UploadLargeAsync(uploadParams);

			return new ImageResponse()
			{
				Status = result.StatusCode,
				Message = result.Error?.Message!,
				Url = result.Uri?.ToString(),
				NameFile = result.OriginalFilename
			};
		}

		public async Task<Dictionary<string, string>> DeleteImages(params string[] nameImages)
		{
			var deleteParams = new DelResParams()
			{
				PublicIds = nameImages.ToList(),
				Type = "upload",
				ResourceType = ResourceType.Image
			};

			var result = await _cloudinary.DeleteResourcesAsync(deleteParams);

			return result.Deleted;
		}

	}
}
