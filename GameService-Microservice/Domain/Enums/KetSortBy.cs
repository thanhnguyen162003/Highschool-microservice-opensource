using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum KetSortBy
    {
        [EnumMember(Value = "Ket name")]
        Name = 1,
        [EnumMember(Value = "Date Created")]
        CreatedAt = 2,
        [EnumMember(Value = "Total Play")]
        TotalPlay = 3,
        [EnumMember(Value = "Total Host")]
        TotalHost = 4
    }
}
