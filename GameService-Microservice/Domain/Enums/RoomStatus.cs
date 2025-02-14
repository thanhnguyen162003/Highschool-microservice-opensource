using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RoomStatus
    {
        [EnumMember(Value = "Waiting")]
        Waiting = 1,
        [EnumMember(Value = "Playing")]
        Playing = 2,
        [EnumMember(Value = "Finished")]
        Finished = 3
    }
}
