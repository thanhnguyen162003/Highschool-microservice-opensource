using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CustomModel
{
    public class UserFlashcardLearningModel
    {
        public Guid UserId { get; set; }
        public Guid FlashcardContentId { get; set; }
        public Guid FlashcardId { get; set; }
        public List<DateTime> LastReviewDateHistory { get; set; } = new List<DateTime>();
        public List<double> TimeSpentHistory { get; set; } = new List<double>();
    }
}
