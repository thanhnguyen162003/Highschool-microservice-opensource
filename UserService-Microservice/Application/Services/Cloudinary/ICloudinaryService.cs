using Application.Common.Models.External;

namespace Application.Service.Cloudinary
{
	public interface ICloudinaryService
	{
		/// <summary>
		/// Uploads a single image to Cloudinary.
		/// </summary>
		/// <param name="file">The large image file to be uploaded.</param>
		/// <returns>A task that represents the asynchronous operation, containing an image response object with details about the uploaded image.</returns>
		Task<ImageResponse> UploadAsync(IFormFile file, string nameImage);

		/// <summary>
		/// Deletes images from Cloudinary by their names.
		/// </summary>
		/// <param name="nameImages">Array of image names that need to be deleted from Cloudinary.</param>
		/// <returns>A dictionary include name of file effect and status of action.</returns>
		Task<Dictionary<string, string>> DeleteImages(params string[] nameImages);
	}
}
