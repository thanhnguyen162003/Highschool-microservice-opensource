using Domain.Enums;

namespace Domain
{
    public static class GlobalConstant
    {
        public static IReadOnlyDictionary<QuestionCategory, int> NumberOfQuestionsFromCategory { get; } = new Dictionary<QuestionCategory, int>
        {
            { QuestionCategory.Lesson, 20 },
            { QuestionCategory.Chapter, 40 },
            { QuestionCategory.Subject, 60 },
            { QuestionCategory.SubjectCurriculum, 60 }
        };

        public static IReadOnlyDictionary<Difficulty, int> PercentOfQuestionsFromDifficulty { get; } = new Dictionary<Difficulty, int>
        {
            { Difficulty.Recognizing, 25 },
            { Difficulty.Comprehensing, 35 },
            { Difficulty.LowLevelApplication, 30 },
            { Difficulty.HighLevelApplication, 10 }
        };
    }
}
