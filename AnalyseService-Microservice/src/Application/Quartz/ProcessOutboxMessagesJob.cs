using Application.Common.Interfaces.KafkaInterface;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Quartz;
using SharedProject.Models;

namespace Application.Quartz;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob(AnalyseDbContext dbContext, ILogger<ProcessOutboxMessagesJob> logger, IKafkaConsumerMethod consumerMethod, IProducerService producerService) : IJob
{
    private readonly AnalyseDbContext _dbContext = dbContext;
    private readonly IKafkaConsumerMethod _consumerMethod = consumerMethod;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger = logger;
    private readonly IProducerService _producerService = producerService;

    //very heavy task
    // public async Task Execute(IJobExecutionContext context)
    // {
    //     CancellationToken stoppingToken = context.CancellationToken;
    //     string topic = TopicKafkaConstaints.UserAnalyseData; 
    //
    //     //get current data
    //     List<UserAnalyseEntity> userData = await _dbContext.UserAnalyseEntity
    //         .Find(Builders<UserAnalyseEntity>.Filter.Empty)
    //         .ToListAsync(stoppingToken);
    //     if (userData.Any())
    //     {
    //         List<RecommendedData> recommendedDatas = new List<RecommendedData>();
    //     //process data
    //     foreach (UserAnalyseEntity user in userData)
    //     {
    //         //get kafka data
    //         List<AnalyseDataDocumentModel>? data =
    //             JsonConvert.DeserializeObject<List<AnalyseDataDocumentModel>>(
    //                 await _consumerMethod.ConsumeByKeyAsync(topic, user.UserId.ToString(), stoppingToken));
    //         if (data is not null)
    //         {
    //             
    //             var topSubjectIds = data
    //                 .Where(d => d.SubjectId.HasValue) // Ensure SubjectId is not null
    //                 .GroupBy(d => d.SubjectId.Value)
    //                 .Select(group => new 
    //                 {
    //                     SubjectId = group.Key
    //                 })
    //                 .Take(4) // Take top 4
    //                 .Select(x => x.SubjectId)
    //                 .ToList();
    //             topSubjectIds.AddRange(user.Subjects);
    //             
    //             var topDocumentIds = data
    //                 .Where(d => d.DocumentId.HasValue) // Ensure DocumentId is not null
    //                 .GroupBy(d => d.DocumentId.Value)
    //                 .Select(group => new 
    //                 {
    //                     DocumentId = group.Key,
    //                     Count = group.Count()
    //                 })
    //                 .OrderByDescending(x => x.Count) // Sort by count descending
    //                 .Take(8) // Take top 8
    //                 .Select(x => x.DocumentId)
    //                 .ToList();
    //             
    //             var topFlashcardIds = data
    //                 .Where(d => d.FlashcardId.HasValue) // Ensure FlashcardId is not null
    //                 .GroupBy(d => d.FlashcardId.Value)
    //                 .Select(group => new 
    //                 {
    //                     FlashcardId = group.Key,
    //                     Count = group.Count()
    //                 })
    //                 .OrderByDescending(x => x.Count) // Sort by count descending
    //                 .Take(8) // Take top 8
    //                 .Select(x => x.FlashcardId)
    //                 .ToList();
    //             
    //             var recommendedData = new RecommendedData
    //             {
    //                 Id = ObjectId.GenerateNewId().ToString(),
    //                 UserId = user.UserId,
    //                 SubjectIds = topSubjectIds,
    //                 DocumentIds = topDocumentIds,
    //                 FlashcardIds = topFlashcardIds,
    //                 Grade = user.Grade,
    //                 TypeExam = user.TypeExam
    //             };
    //             recommendedDatas.Add(recommendedData);
    //             await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataRecommended, recommendedData.UserId.ToString(),recommendedData);
    //         }
    //         await _dbContext.RecommendedData.InsertManyAsync(recommendedDatas);
    //     }
    //     }
    //     _logger.LogInformation("nothing to process");
    // }
    public async Task Execute(IJobExecutionContext context)
    {
        CancellationToken stoppingToken = context.CancellationToken;
        string topic = TopicKafkaConstaints.UserAnalyseData;

        // Get current data
        List<UserAnalyseEntity> userData = await _dbContext.UserAnalyseEntity
            .Find(Builders<UserAnalyseEntity>.Filter.Empty)
            .ToListAsync(stoppingToken);

        if (userData.Any())
        {
            List<RecommendedData> recommendedDatas = new List<RecommendedData>();

            // Process data
            //foreach (UserAnalyseEntity user in userData)
            //{
            //    // Get Kafka data
            //    List<AnalyseDataDocumentModel>? data = await _consumerMethod.ConsumeByKeyAsync(topic, user.UserId.ToString(), stoppingToken);

            //        if (data is not null)
            //        {
            //            var topSubjectIds = data
            //                .Where(d => d.SubjectId.HasValue)
            //                .GroupBy(d => d.SubjectId!.Value)
            //                .Select(group => new { SubjectId = group.Key })
            //                .Take(4)
            //                .Select(x => x.SubjectId)
            //                .ToList();
            //            topSubjectIds.AddRange(user.Subjects);

            //            var topDocumentIds = data
            //                .Where(d => d.DocumentId.HasValue)
            //                .GroupBy(d => d.DocumentId!.Value)
            //                .Select(group => new { DocumentId = group.Key, Count = group.Count() })
            //                .OrderByDescending(x => x.Count)
            //                .Take(8)
            //                .Select(x => x.DocumentId)
            //                .ToList();

            //            var topFlashcardIds = data
            //                .Where(d => d.FlashcardId.HasValue)
            //                .GroupBy(d => d.FlashcardId!.Value)
            //                .Select(group => new { FlashcardId = group.Key, Count = group.Count() })
            //                .OrderByDescending(x => x.Count)
            //                .Take(8)
            //                .Select(x => x.FlashcardId)
            //                .ToList();

            //            // Create the recommended data object
            //            var recommendedData = new RecommendedData
            //            {
            //                Id = ObjectId.GenerateNewId().ToString(),
            //                UserId = user.UserId,
            //                SubjectIds = topSubjectIds,
            //                DocumentIds = topDocumentIds,
            //                FlashcardIds = topFlashcardIds,
            //                Grade = user.Grade,
            //                TypeExam = user.TypeExam
            //            };
            //            // Add to the batch list
            //            recommendedDatas.Add(recommendedData);
            //        }
            //    }

            // Send batch messages to Kafka
            if (recommendedDatas.Any())
            {
                foreach (var recommendedData in recommendedDatas)
                {
                    // Send each recommended data in the batch
                    await _producerService.ProduceObjectWithKeyAsyncBatch(TopicKafkaConstaints.DataRecommended, recommendedData.UserId.ToString(), recommendedData);
                }
                await _producerService.FlushedData(TimeSpan.FromSeconds(10));
                await _dbContext.RecommendedData.InsertManyAsync(recommendedDatas);
            }
        }

        _logger.LogInformation("Processing complete");
    }
}
