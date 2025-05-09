using Application.Common.Models.StarredTermModel;
using Application.Common.Models.StudiableTermModel;
using Domain.Entities;
using Domain.Enums;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Application.Common.Models.ContainerModel
{
	public class ContainerResponseModel
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
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
        public string[] StarredTerms { get; set; }
		public int CardsPerDay { get; set; }
		public Guid? PresetId { get; set; }
        public List<StudiableTermResponseModel>? StudiableTerms { get; set; } = new List<StudiableTermResponseModel>();
    }
}
