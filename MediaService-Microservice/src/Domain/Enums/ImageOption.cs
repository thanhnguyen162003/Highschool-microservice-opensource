using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Domain.Enums;
public class ImageOption
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ImageFormat
    {
        jpg,
        png,
        gif,
        bmp,
        tiff,
        webp
    }

    public enum ImageSize
    {
        Avatar,
        Thumbnail,
        Another
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TypeResize
    {
        Crop,
        Pad,
        BoxPad,
        Max,
        Min,
        Stretch,
        Manual
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ImageFolder
    {
        // HighSchool paths
        [Description("HighSchool/avatars")]
        HighSchoolAvatar,

        [Description("HighSchool/contents")]
        HighSchoolFlashcardContent,

        [Description("HighSchool/subjects")]
        HighSchoolSubject,

        [Description("HighSchool/theories")]
        HighSchoolTheory,

        [Description("HighSchool/news/contents")]
        HighSchoolNewsContent,

        [Description("HighSchool/news/thumbnails")]
        HighSchoolNewsThumbnail,

        // Game paths
        [Description("Game/avatars")]
        GameAvatar,

        [Description("Game/kets/thumbnails")]
        GameKetThumbnail,

        [Description("Game/kets/contents")]
        GameKetContent,

        [Description("test")]
        Test,

        [Description("HighSchool/careers/images")]
        HighSchoolCareer,
    }

}
