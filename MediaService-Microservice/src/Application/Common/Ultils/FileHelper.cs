using Application.Common.Extentions;
using Application.Common.Models.ImageModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using static Domain.Enums.ImageOption;

namespace Application.Common.Ultils;

public static class FileHelper
{
    public static byte[] ResizeImage(PresetImage preset, IFormFile file)
    {
        using var outputStream = new MemoryStream();
        var format = GetEncoder(preset.Format);
        using (var image = Image.Load(file.OpenReadStream()))
        {
            image.Mutate(c => c.Resize(new ResizeOptions()
            {
                Size = new Size((int)(preset.Width ?? image.Width), (int)(preset.Height ?? image.Height)),
                Mode = (ResizeMode)(preset.TypeResize.HasValue ? preset.TypeResize?.GetEnum<ResizeMode>() : ResizeMode.Manual)!,
                CenterCoordinates = GetCenterCoordinates(preset, image)
            }));

            if(format == null)
            {
                try
                {
                    image.Save(outputStream, image.Metadata.DecodedImageFormat!);
                }
                catch
                {
                    image.Save(outputStream, new PngEncoder());
                }
            }
            else
            {
                image.Save(outputStream, format);
            }

        }

        return outputStream.ToArray();
    }

    public static Point GetCenterCoordinates(PresetImage preset, Image image)
    {
        return preset.TypeResize.Equals(TypeResize.Crop) ? new Point((int)(preset.Width ?? image.Width) / 2, (int)(preset.Height ?? image.Height) / 2) : Point.Empty;
    }

    public static IImageEncoder? GetEncoder(ImageFormat? format)
    {
        switch (format)
        {
            case ImageFormat.jpg:
                return new JpegEncoder();
            case ImageFormat.png:
                return new PngEncoder();
            case ImageFormat.gif:
                return new GifEncoder();
            case ImageFormat.bmp:
                return new BmpEncoder();
            case ImageFormat.tiff:
                return new TiffEncoder();
            case ImageFormat.webp:
                return new WebpEncoder();
            default:
                return null;
        }
    }
}
