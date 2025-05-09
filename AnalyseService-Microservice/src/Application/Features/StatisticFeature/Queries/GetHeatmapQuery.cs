using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.StatisticModel;
using Infrastructure.Data;
using MongoDB.Driver;


namespace Application.Features.StatisticFeature.Queries
{
    public class GetHeatmapQuery : IRequest<HeatmapModel>
    {
        public string ViewType { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
    }

    public class GetHeatmapQueryHandler(AnalyseDbContext dbContext, IMapper _mapper, IClaimInterface claimInterface) : IRequestHandler<GetHeatmapQuery, HeatmapModel>
    {
        public async Task<HeatmapModel> Handle(GetHeatmapQuery request, CancellationToken cancellationToken)
        {
            var userId = claimInterface.GetCurrentUserId;
            var startDate = new DateTime(request.StartYear,1,1,0,0,0);
            var endDate = new DateTime(request.EndYear,12,31,23,59,59);
            int totalCount = 0;

            List<HeatmapData> heatmapData = new List<HeatmapData>();
            HeatmapModel response = new HeatmapModel();
            switch (request.ViewType.ToLower())
            {
                case "flashcard":
                    var flashcardLearning = await dbContext.UserFlashcardLearningModel.Find(x => x.UserId == userId && x.LearningDates.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    heatmapData = flashcardLearning
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate) 
                        .GroupBy(date => date.Date) 
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"), 
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date) 
                        .ToList();
                    
                    var learningDates = flashcardLearning
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate);  
                    totalCount = learningDates.Count();
                    response = new HeatmapModel
                    {
                        TotalActivity = totalCount,
                        VỉewType = request.ViewType,
                        StartYear = request.StartYear,
                        EndYear = request.EndYear,
                        Data = heatmapData
                    };  
                    break;

                case "login":
                    var login = await dbContext.UserRetentionModel.Find(x => x.UserId == userId && x.LoginDate.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    heatmapData = login
                        .SelectMany(x => x.LoginDate)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList();

                    var loginDates = login
                        .SelectMany(x => x.LoginDate)
                        .Where(date => date >= startDate && date <= endDate);
                    totalCount = loginDates.Count();
                    response = new HeatmapModel
                    {
                        TotalActivity = totalCount,
                        VỉewType = request.ViewType,
                        StartYear = request.StartYear,
                        EndYear = request.EndYear,
                        Data = heatmapData
                    };
                    break;

                case "flashcard/login":
                    flashcardLearning = await dbContext.UserFlashcardLearningModel.Find(x => x.UserId == userId && x.LearningDates.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    login = await dbContext.UserRetentionModel.Find(x => x.UserId == userId && x.LoginDate.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    heatmapData.AddRange(login
                        .SelectMany(x => x.LoginDate)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList());
                    heatmapData.AddRange(flashcardLearning
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList());

                    loginDates = login
                        .SelectMany(x => x.LoginDate)
                        .Where(date => date >= startDate && date <= endDate);
                    learningDates = flashcardLearning
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate);
                    totalCount = loginDates.Count() + learningDates.Count();
                    response = new HeatmapModel
                    {
                        TotalActivity = totalCount,
                        VỉewType = request.ViewType,
                        StartYear = request.StartYear,
                        EndYear = request.EndYear,
                        Data = heatmapData
                    };
                    break;

                case "learnedlesson":
                    var lesson = await dbContext.UserLessonLearningModel.Find(x => x.UserId == userId && x.LearningDates.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    heatmapData = lesson
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList();

                    var lessonDates = lesson
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate);
                    totalCount = lessonDates.Count();
                    response = new HeatmapModel
                    {
                        TotalActivity = totalCount,
                        VỉewType = request.ViewType,
                        StartYear = request.StartYear,
                        EndYear = request.EndYear,
                        Data = heatmapData
                    };
                    break;

                case "flashcard/learnedlesson":
                    flashcardLearning = await dbContext.UserFlashcardLearningModel.Find(x => x.UserId == userId && x.LearningDates.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    lesson = await dbContext.UserLessonLearningModel.Find(x => x.UserId == userId && x.LearningDates.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    heatmapData.AddRange(lesson
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList());
                    heatmapData.AddRange(flashcardLearning
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList());

                    lessonDates = lesson
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate);
                    learningDates = flashcardLearning
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate);
                    totalCount = lesson.Count() + learningDates.Count();
                    response = new HeatmapModel
                    {
                        TotalActivity = totalCount,
                        VỉewType = request.ViewType,
                        StartYear = request.StartYear,
                        EndYear = request.EndYear,
                        Data = heatmapData
                    };
                    break;

                case "login/learnedlesson":
                    login = await dbContext.UserRetentionModel.Find(x => x.UserId == userId && x.LoginDate.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    lesson = await dbContext.UserLessonLearningModel.Find(x => x.UserId == userId && x.LearningDates.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    heatmapData.AddRange(lesson
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList());
                    heatmapData.AddRange(login
                        .SelectMany(x => x.LoginDate)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList());

                    lessonDates = lesson
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate);
                    loginDates = login
                        .SelectMany(x => x.LoginDate)
                        .Where(date => date >= startDate && date <= endDate);
                    totalCount = lesson.Count() + loginDates.Count();
                    response = new HeatmapModel
                    {
                        TotalActivity = totalCount,
                        VỉewType = request.ViewType,
                        StartYear = request.StartYear,
                        EndYear = request.EndYear,
                        Data = heatmapData
                    };
                    break;

                case "flashcard/login/learnedlesson":
                    flashcardLearning = await dbContext.UserFlashcardLearningModel.Find(x => x.UserId == userId && x.LearningDates.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    lesson = await dbContext.UserLessonLearningModel.Find(x => x.UserId == userId && x.LearningDates.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    login = await dbContext.UserRetentionModel.Find(x => x.UserId == userId && x.LoginDate.Any(date => date >= startDate && date <= endDate)).ToListAsync();
                    heatmapData.AddRange(login
                        .SelectMany(x => x.LoginDate)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList());
                    heatmapData.AddRange(lesson
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList());
                    heatmapData.AddRange(flashcardLearning
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate)
                        .GroupBy(date => date.Date)
                        .Select(group => new HeatmapData
                        {
                            Date = group.Key.ToString("yyyy-MM-dd"),
                            Count = group.Count()
                        })
                        .OrderBy(data => data.Date)
                        .ToList());
                    loginDates = login
                        .SelectMany(x => x.LoginDate)
                        .Where(date => date >= startDate && date <= endDate);
                    lessonDates = lesson
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate);
                    learningDates = flashcardLearning
                        .SelectMany(x => x.LearningDates)
                        .Where(date => date >= startDate && date <= endDate);
                    totalCount = lesson.Count() + learningDates.Count() + loginDates.Count();
                    response = new HeatmapModel
                    {
                        TotalActivity = totalCount,
                        VỉewType = request.ViewType,
                        StartYear = request.StartYear,
                        EndYear = request.EndYear,
                        Data = heatmapData
                    };
                    break;

            }            
           

            
            return response;

        }
    }
}
