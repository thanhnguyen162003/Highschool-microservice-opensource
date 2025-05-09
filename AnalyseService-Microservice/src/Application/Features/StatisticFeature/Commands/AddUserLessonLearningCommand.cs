using System.Net;
using Application.Common.Models.DaprModel.Enrollment;
using Application.Common.Models.SearchModel;
using Dapr.Client;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Application.Features.StatisticFeature.Commands;

public record AddUserLessonLearningCommand : IRequest<ResponseModel>
{
}

public class AddUserLessonLearningCommandHandler(
    IMapper mapper,
    AnalyseDbContext dbContext,
    DaprClient daprClient,
    ILogger<AddUserLessonLearningCommandHandler> logger)
    : IRequestHandler<AddUserLessonLearningCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(AddUserLessonLearningCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await daprClient.InvokeMethodAsync<EnrollmentResponseDapr>(
               HttpMethod.Get,
               "document-sidecar",
               $"api/v1/dapr/enrollment"
           );
            var today = DateTime.Today; // Get current date for comparison

            foreach (var enrollment in response.Enrollment)
            {
                var lessonLearnDates = enrollment.LessonLearnDate; // repeated string from proto
                if (lessonLearnDates.Count == 0) continue;

                var userIdGuid = Guid.Parse(enrollment.UserId);
                var filter = Builders<UserLessonLearningModel>.Filter.Eq(u => u.UserId, userIdGuid);
                var existingUser = await dbContext.UserLessonLearningModel.Find(filter).FirstOrDefaultAsync();

                // Convert lessonLearnDates strings to DateTime and filter today's lessons
                var parsedDates = lessonLearnDates.Select(d => DateTime.Parse(d)).ToList();
                var todayLessonsCount = parsedDates.Count(d => d.Date == today);

                if (existingUser != null)
                {
                    // Calculate the difference based on today's count
                    int oldToday = existingUser.TodayLessonsLearned;
                    int diff = todayLessonsCount - oldToday;

                    if (diff != 0 || !existingUser.LearningDates.SequenceEqual(parsedDates))
                    {
                        var update = Builders<UserLessonLearningModel>.Update
                            .Set(u => u.TodayLessonsLearned, todayLessonsCount)
                            .Set(u => u.LearningDates, parsedDates)
                            .Inc(u => u.TotalLessonsLearned, diff);

                        await dbContext.UserLessonLearningModel.UpdateOneAsync(filter, update);
                    }
                }
                else
                {
                    // Create new user record
                    var newUser = new UserLessonLearningModel
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = userIdGuid,
                        TodayLessonsLearned = todayLessonsCount,
                        TotalLessonsLearned = parsedDates.Count, // Total is all lessons, not just today's
                        LearningDates = parsedDates
                    };

                    await dbContext.UserLessonLearningModel.InsertOneAsync(newUser);
                }
            }
            return new ResponseModel(HttpStatusCode.Created, "thành công");
        }
        catch (Exception e)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, e.Message);
        }
    }
    public bool AreRecordsEqual(UserFlashcardLearningModel dbRecord, UserFlashcardLearningModel newRecord)
    {
        return dbRecord.FlashcardId == newRecord.FlashcardId &&
               dbRecord.FlashcardContentId == newRecord.FlashcardContentId &&
               dbRecord.UserId == newRecord.UserId &&
               dbRecord.LearningDates.Count == newRecord.LearningDates.Count &&
                dbRecord.LearningDates.SequenceEqual(newRecord.LearningDates)
               ;
    }
}
