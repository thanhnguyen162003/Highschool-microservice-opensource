using System.ComponentModel;
using System.Reflection;
using static Domain.Enums.ImageOption;

namespace Application.Common.Extentions;

public static class EnumExtention
{
    public static string GetDescription(this Enum path)
    {
        var field = path.GetType().GetField(path.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? path.ToString();
    }

    public static T GetEnum<T>(this string obj) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), obj, true);
    }

    public static T GetEnum<T>(this Enum obj) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), obj.ToString(), true);
    }
}
