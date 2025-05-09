using System.Text.Json.Serialization;

namespace Domain.Enums
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum StudySetAnswerMode
	{
		Word,
		Definition,
		Both
	}
}
