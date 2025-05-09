using Domain.Enums;

namespace Domain.Entities
{
	public class Container
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid FlashcardId { get; set; }
		public DateTime ViewAt { get; set; }
		public bool ShuffleFlashcards { get; set; }
		public int? LearnRound { get; set; }
		public LearnMode? LearnMode { get; set; }
		public bool ShuffleLearn { get; set; }
		public bool StudyStarred { get; set; }
		public StudySetAnswerMode? AnswerWith { get; set; }
		public MultipleAnswerMode? MultipleAnswerMode { get; set; }
		public bool ExtendedFeedbackBank { get; set; }	
		public bool EnableCardsSorting { get; set; }
		public int? CardsRound { get; set; }
		public bool CardsStudyStarred { get; set; }
		public LimitedStudySetAnswerMode? CardsAnswerWith { get; set; }
		public bool MatchStudyStarred { get; set; }
		public double Retrievability { get; set; }
		public double[] FsrsParameters { get; set; } = new double[19];
		public int CardsPerDay { get; set; } = 20;
        public virtual List<StudiableTerm>? StudiableTerms { get; set; } = new List<StudiableTerm>();
		public virtual List<StarredTerm>? StarredTerms { get; set; } = new List<StarredTerm>();
		public virtual Flashcard? Flashcard { get; set; }
	}
}
