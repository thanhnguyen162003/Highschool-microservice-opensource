
using System.Text.Json.Serialization;

namespace Domain.Enumerations
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ReportType
    {
		Flashcard,
		FlashcardContent,
		Comment,
		Subject,
		Chapter,
		Lesson,
		Document,
		Quiz,
		Test
    }
}
