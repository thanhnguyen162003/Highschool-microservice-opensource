using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FlashcardType
    {
        Lesson,         // Thuộc về một bài học
        Chapter,        // Thuộc về một chương
        Subject,        // Thuộc về một môn học
        SubjectCurriculum  // Thuộc về một chương trình môn học
    }
} 