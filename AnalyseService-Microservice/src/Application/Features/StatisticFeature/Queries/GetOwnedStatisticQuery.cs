using System.Collections.Generic;
using System.Threading;
using Amazon.Runtime.Internal.Transform;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.StatisticModel;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Application.UserServiceRpc;

namespace Application.Features.StatisticFeature.Queries
{
    public class GetOwnedStatisticQuery : IRequest<OwnedStatistic>
    {
    }

    public class GetOwnedStatisticQueryHandler(AnalyseDbContext dbContext, IMapper _mapper, IClaimInterface claimInterface) : IRequestHandler<GetOwnedStatisticQuery, OwnedStatistic>
    {
        public async Task<OwnedStatistic> Handle(GetOwnedStatisticQuery request, CancellationToken cancellationToken)
        {
            var userId = claimInterface.GetCurrentUserId;
            var retention = await dbContext.UserRetentionModel.Find(x => x.UserId == userId).SingleOrDefaultAsync();
            var flashcardLearning = await dbContext.UserFlashcardLearningModel.Find(x => x.UserId == userId).ToListAsync();
            var lesson = await dbContext.UserLessonLearningModel.Find(x => x.UserId == userId).SingleOrDefaultAsync();
            var response = new OwnedStatistic()
            {
                CurrentLoginStreak = retention?.CurrentStreak ?? 0,
                LongestLoginStreak = retention?.MaxStreak ?? 0,
                TodayLessonLearned = lesson?.TodayLessonsLearned ?? 0,
                TotalLessonLearned = lesson?.TotalLessonsLearned ?? 0,
                TotalFlashcardLearned = flashcardLearning?.Select(x => x?.FlashcardId).Distinct().Count() ?? 0,
                TotalFlashcardContentLearned = flashcardLearning?.Select(x => x?.FlashcardContentId).Distinct().Count() ?? 0,
                CurrentLearnStreak = CalculateCurrentStreak(flashcardLearning?.SelectMany(x => x?.LearningDates ?? Enumerable.Empty<DateTime>()).ToList() ?? new List<DateTime>()),
                LongestLearnStreak = CalculateMaxStreak(flashcardLearning?.SelectMany(x => x?.LearningDates ?? Enumerable.Empty<DateTime>()).ToList() ?? new List<DateTime>()),
                TotalFlashcardContentHours = flashcardLearning?.SelectMany(x => x?.TimeSpentHistory ?? Enumerable.Empty<double>()).Sum() ?? 0,
                TotalFlashcardLearnDates = CalculateDateAmount(flashcardLearning?.SelectMany(x => x?.LearningDates ?? Enumerable.Empty<DateTime>()).ToList() ?? new List<DateTime>())
            };
            return response;

        }
        private int CalculateDateAmount(List<DateTime> sortedLogins)
        {
            int totalUniqueDays = sortedLogins.Select(x => x.Date).Distinct().Count();

            return totalUniqueDays;
        }
        private int CalculateCurrentStreak(List<DateTime> sortedLogins)
        {
            int streak = 0;
            DateTime today = DateTime.Today;
            var uniqueDates = sortedLogins.Select(x => x.Date).Distinct().OrderBy(x => x).ToList();

            for (int i = uniqueDates.Count - 1; i >= 0; i--)
            {
                if ((today.Date - uniqueDates[i].Date).TotalDays == streak)
                {
                    streak++;
                }
                else
                {
                    break;
                }
            }

            return streak;
        }

        private int CalculateMaxStreak(List<DateTime> sortedLogins)
        {
            if (sortedLogins == null || sortedLogins.Count == 0) return 0;

            int maxStreak = 1, currentStreak = 1;
            var uniqueDates = sortedLogins.Select(x => x.Date).Distinct().OrderBy(x => x).ToList();
            for (int i = 1; i < uniqueDates.Count; i++)
            {
                if ((uniqueDates[i].Date - uniqueDates[i - 1].Date).TotalDays == 1)
                {
                    currentStreak++;
                }
                else
                {
                    maxStreak = Math.Max(maxStreak, currentStreak);
                    currentStreak = 1; // Reset streak
                }
            }

            return Math.Max(maxStreak, currentStreak);
        }
    }

}
