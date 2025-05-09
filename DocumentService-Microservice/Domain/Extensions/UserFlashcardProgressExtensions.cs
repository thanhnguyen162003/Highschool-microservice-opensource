using Domain.Entities;

namespace Domain.Extensions
{
    public static class UserFlashcardProgressExtensions
    {
        /// <summary>
        /// Cập nhật LastReviewDate và lưu giá trị cũ vào lịch sử
        /// </summary>
        public static void UpdateLastReviewDate(this UserFlashcardProgress progress, DateTime? newDate)
        {
            // Nếu đã có LastReviewDate cũ, thêm vào lịch sử
            if (progress.LastReviewDate.HasValue)
            {
                progress.LastReviewDateHistory.Add(progress.LastReviewDate.Value);
            }
            
            // Cập nhật LastReviewDate mới
            progress.LastReviewDate = newDate;
        }
        
        /// <summary>
        /// Cập nhật TimeSpent và lưu giá trị cũ vào lịch sử
        /// </summary>
        public static void UpdateTimeSpent(this UserFlashcardProgress progress, double newTimeSpent)
        {
            progress.TimeSpentHistory.Add(newTimeSpent);
            
            // Cập nhật giá trị TimeSpent mới
            progress.TimeSpent = newTimeSpent;
        }
    }
} 