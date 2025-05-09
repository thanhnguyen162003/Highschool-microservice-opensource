using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Enums
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum LimitedStudySetAnswerMode
	{
		Term,
		Definition
	}
}
