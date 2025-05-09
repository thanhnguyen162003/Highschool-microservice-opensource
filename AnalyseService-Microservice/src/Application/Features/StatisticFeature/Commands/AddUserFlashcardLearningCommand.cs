using System.Linq;
using System.Net;
using Application.Common.Models.DaprModel.Flashcard;
using Application.Common.Models.RoadmapDataModel;
using Application.Common.Models.SearchModel;
using Dapr.Client;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Features.StatisticFeature.Commands;

public record AddUserFlashcardLearningCommand : IRequest<ResponseModel>
{
}

public class AddUserFlashcardLearningCommandHandler(
    IMapper mapper,
    AnalyseDbContext dbContext,
    DaprClient daprClient,
    ILogger<AddUserFlashcardLearningCommandHandler> logger)
    : IRequestHandler<AddUserFlashcardLearningCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(AddUserFlashcardLearningCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var list = await daprClient.InvokeMethodAsync<UserFlashcardLearningResponseDapr>(
            HttpMethod.Get,
            "document-sidecar",
            $"api/v1/dapr/user-flashcard-learning"
            );

            var response = list.UserFlashcardLearning
                .GroupBy(x => new { x.FlashcardId, x.FlashcardContentId })
                .Select(group => new UserFlashcardLearningModel
                {
                    FlashcardId = Guid.Parse(group.Key.FlashcardId),
                    FlashcardContentId = Guid.Parse(group.Key.FlashcardContentId),
                    LearningDates = group
                        .SelectMany(x => x.LastReviewDateHistory.Select(DateTime.Parse))
                        .OrderBy(date => date)
                        .ToList(),
                    UserId = Guid.Parse(group.First().UserId),
                    TimeSpentHistory = group.SelectMany(x => x.TimeSpentHistory).ToList()
                })
                .ToList();

            var flashcardIds = response.Select(x => x.FlashcardId).ToList();
            var flashcardContentIds = response.Select(x => x.FlashcardContentId).ToList();

            var dbRecords = await dbContext.UserFlashcardLearningModel
                .Find(x => flashcardIds.Contains(x.FlashcardId) && flashcardContentIds.Contains(x.FlashcardContentId))
                .ToListAsync();

            // Convert DB records into a dictionary for quick lookup
            var dbRecordsDict = dbRecords.ToDictionary(x => (x.FlashcardId, x.FlashcardContentId, x.UserId));

            // Lists for new inserts and updates
            List<UserFlashcardLearningModel> newRecords = new();
            List<ReplaceOneModel<UserFlashcardLearningModel>> updates = new();

            foreach (var record in response)
            {
                var key = (record.FlashcardId, record.FlashcardContentId, record.UserId);

                if (dbRecordsDict.TryGetValue(key, out var existingRecord))
                {
                    // Check if data is different
                    if (!AreRecordsEqual(existingRecord, record))
                    {
                        record.Id = existingRecord.Id;
                        updates.Add(new ReplaceOneModel<UserFlashcardLearningModel>(
                            Builders<UserFlashcardLearningModel>.Filter.Eq(x => x.Id, existingRecord.Id),
                            record)
                        {
                            IsUpsert = false // Only update existing records
                        });
                    }
                }
                else
                {
                    // If record doesn't exist, add it as a new entry
                    newRecords.Add(record);
                }
            }

            // Batch update existing records
            if (updates.Any())
            {
                await dbContext.UserFlashcardLearningModel.BulkWriteAsync(updates);
            }

            // Insert new records
            if (newRecords.Any())
            {
                await dbContext.UserFlashcardLearningModel.InsertManyAsync(newRecords);
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
