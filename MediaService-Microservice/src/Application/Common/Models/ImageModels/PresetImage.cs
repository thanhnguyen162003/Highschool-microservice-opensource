using static Domain.Enums.ImageOption;

namespace Application.Common.Models.ImageModels;

public class PresetImage
{
    public string? Name { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public TypeResize? TypeResize { get; set; }
    public ImageFormat? Format { get; set; }

    public PresetImage GetPreset()
    {
        switch (Name)
        {
            case "avatar":
                {
                    this.Width = 200;
                    this.Height = 200;
                    this.TypeResize = Domain.Enums.ImageOption.TypeResize.Crop;
                    this.Format = ImageFormat.webp;

                    return this;
                }
                
            case "thumbnail":
                {
                    this.Width = 1280;
                    this.Height = 720;
                    this.TypeResize = Domain.Enums.ImageOption.TypeResize.Crop;
                    this.Format = ImageFormat.png;

                    return this;
                }

            default:
                return this;
        }
    }

    public bool IsNull()
    {
        return Name == null && Width == null && Height == null && TypeResize == null && Format == null;
    }
}
