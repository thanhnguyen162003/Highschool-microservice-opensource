using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AvatarGroup
    {
        Rarity = 1,
        Type = 2
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AvatarSortBy
    {
        name = 1,
        rarity = 2,
        type = 3
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AvatarRarity
    {
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Epic = 4,
        Legendary = 5,
        Chroma = 6,
        Unique = 7,
        Mythic = 8,
        Limited = 9
    }
}
