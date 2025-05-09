

using Application.Common.Models.StatisticModel;
using Humanizer;
using Infrastructure.Repositories.Interfaces;
using System.Collections.Generic;

namespace Application.Features.StatisticFeature.Queries
{
    public class ContentCreationQuery : IRequest<List<ContentCreationResponseModel>>
    {        
        public string Type { get; set; }
    }

    public class ContentCreationQueryHandler(IUnitOfWork unitOfWork, IMapper _mapper) : IRequestHandler<ContentCreationQuery, List<ContentCreationResponseModel>>
    {
        public async Task<List<ContentCreationResponseModel>> Handle(ContentCreationQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            switch (request.Type.ToLower())
            {
                case "year":
                    //var startDate = new DateTime(DateTime.UtcNow.AddYears(-1).Year, DateTime.UtcNow.AddYears(-1).Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    var startDate = new DateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    //int daysInMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);

                    //var endDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, daysInMonth, 23, 59, 59, DateTimeKind.Utc);
                    var endDate = new DateTime(DateTime.UtcNow.Year, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                    var flashcards = await unitOfWork.FlashcardRepository.GetFlashcardsCountByDay(startDate, endDate, cancellationToken);
                    var documents = await unitOfWork.DocumentRepository.GetDocumentsCountByDay(startDate, endDate, cancellationToken);

                    var results = new List<ContentCreationResponseModel>();

                    for (int i = 0; i <= 11; i++)
                    {
                        var date = startDate.AddMonths(i);

                        results.Add(new ContentCreationResponseModel
                        {
                            Date = date,
                            Flashcards = flashcards.Where(x => x.Key.Year == date.Year && x.Key.Month == date.Month).Sum(x => x.Value),
                            Documents = documents.Where(x => x.Key.Year == date.Year && x.Key.Month == date.Month).Sum(x => x.Value),
                            Courses = 0 // Assuming you don't have course data yet
                        });
                    }
                    return results;

                case "month":
                    //startDate = new DateTime(DateTime.UtcNow.AddMonths(-1).Year, DateTime.UtcNow.AddMonths(-1).Month, DateTime.UtcNow.AddMonths(-1).Day, 0, 0, 0, DateTimeKind.Utc);
                    startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    //endDate = DateTime.SpecifyKind(now, DateTimeKind.Utc);
                    int daysInMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
                    endDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, daysInMonth, 23, 59, 59, DateTimeKind.Utc);
                    int numberOfDays = (endDate - startDate).Days;

                    flashcards = await unitOfWork.FlashcardRepository.GetFlashcardsCountByDay(startDate, endDate, cancellationToken);
                    documents = await unitOfWork.DocumentRepository.GetDocumentsCountByDay(startDate, endDate, cancellationToken);

                    results = new List<ContentCreationResponseModel>();

                    for (int i = 0; i <= numberOfDays; i++)
                    {
                        var date = startDate.AddDays(i);

                        results.Add(new ContentCreationResponseModel
                        {
                            Date = date,
                            Flashcards = flashcards.ContainsKey(date) ? flashcards[date] : 0,
                            Documents = documents.ContainsKey(date) ? documents[date] : 0,
                            Courses = 0 // Assuming you don't have course data yet
                        });
                    }
                    return results;

                case "week":
                    int daysToMonday = ((int)now.DayOfWeek + 6) % 7; // Days to subtract to reach Monday
                    var currentWeekMonday = now.AddDays(-daysToMonday).Date;
                    var currentWeekSunday = currentWeekMonday.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59); // 2025-03-23 23:59:59

                    //startDate = new DateTime(DateTime.UtcNow.AddDays(-7).Year, DateTime.UtcNow.AddDays(-7).Month, DateTime.UtcNow.AddDays(-7).Day,0,0,0, DateTimeKind.Utc);
                    //endDate = DateTime.SpecifyKind(now, DateTimeKind.Utc);
                    startDate = currentWeekMonday; // Start from 'amount' weeks ago
                    endDate = currentWeekSunday; // End before current week

                    flashcards = await unitOfWork.FlashcardRepository.GetFlashcardsCountByDay(startDate,endDate,cancellationToken);
                    documents = await unitOfWork.DocumentRepository.GetDocumentsCountByDay(startDate,endDate,cancellationToken);

                    results = new List<ContentCreationResponseModel>();

                    for (int i = 0; i <= 6; i++)
                    {
                        var date = startDate.AddDays(i); 

                        results.Add(new ContentCreationResponseModel
                        {
                            Date = date,
                            Flashcards = flashcards.ContainsKey(date) ? flashcards[date] : 0,
                            Documents = documents.ContainsKey(date) ? documents[date] : 0,
                            Courses = 0 // Assuming you don't have course data yet
                        });
                    }
                    return results;

                case "day":
                    startDate = new DateTime(DateTime.UtcNow.AddHours(-24).Year, DateTime.UtcNow.AddHours(-24).Month, DateTime.UtcNow.AddHours(-24).Day, DateTime.UtcNow.AddHours(-24).Hour, 0, 0, DateTimeKind.Utc);
                    startDate = DateTime.SpecifyKind(now, DateTimeKind.Utc).AddHours(-24);
                    endDate = DateTime.SpecifyKind(now, DateTimeKind.Utc);

                    flashcards = await unitOfWork.FlashcardRepository.GetFlashcardsCountByTime(startDate, endDate, cancellationToken);
                    documents = await unitOfWork.DocumentRepository.GetDocumentsCountByTime(startDate, endDate, cancellationToken);

                    results = new List<ContentCreationResponseModel>();

                    for (int i = 0; i <= 23; i++)
                    {
                        var date = startDate.AddHours(i);

                        results.Add(new ContentCreationResponseModel
                        {
                            Date = date,
                            Flashcards = flashcards.Where(x => x.Key.Year == date.Year && x.Key.Month == date.Month && x.Key.Day == date.Day && x.Key.Hour == date.Hour).Sum(x => x.Value),
                            Documents = documents.Where(x => x.Key.Year == date.Year && x.Key.Month == date.Month && x.Key.Day == date.Day && x.Key.Hour == date.Hour).Sum(x => x.Value),
                            Courses = 0 // Assuming you don't have course data yet
                        });
                    }
                    return results;

            }
            return null;
        }
    }
}
