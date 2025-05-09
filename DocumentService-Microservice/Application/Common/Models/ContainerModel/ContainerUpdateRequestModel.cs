using Domain.Enums;

namespace Application.Common.Models.ContainerModel
{
	public class ContainerUpdateRequestModel
	{
		public bool? ShuffleFlashcards { get; set; }
		public int? LearnRound { get; set; }
		public LearnMode? LearnMode { get; set; }
		public bool? ShuffleLearn { get; set; }
		public bool? StudyStarred { get; set; }
		public StudySetAnswerMode? AnswerWith { get; set; }
		public MultipleAnswerMode? MultipleAnswerMode { get; set; }
		public bool? ExtendedFeedbackBank { get; set; }
		public bool? EnableCardsSorting { get; set; }
		public int? CardsRound { get; set; }
		public bool? CardsStudyStarred { get; set; }
		public LimitedStudySetAnswerMode? CardsAnswerWith { get; set; }
		public bool? MatchStudyStarred { get; set; }
		public int? CardsPerDay { get; set; }
	}
}
