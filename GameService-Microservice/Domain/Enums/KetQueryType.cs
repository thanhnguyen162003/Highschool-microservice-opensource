using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum KetQueryType
    {
        [EnumMember(Value = "Get all kets")]
        All = 1,
        [EnumMember(Value = "Get owner kets")]
        MyKet = 2
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum KetCategoryQuery
    {
        NewUpdate = 1,
        Recommended = 2,
        TopKet = 3
    }
}
