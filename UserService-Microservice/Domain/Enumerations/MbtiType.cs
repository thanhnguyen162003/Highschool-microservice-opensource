using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Text.Json.Serialization;

namespace Domain.Enumerations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MBTIType
	{

		INTJ, INTP, ENTJ, ENTP,
		INFJ, INFP, ENFJ, ENFP,
		ISTJ, ISFJ, ESTJ, ESFJ,
		ISTP, ISFP, ESTP, ESFP
	}
}
