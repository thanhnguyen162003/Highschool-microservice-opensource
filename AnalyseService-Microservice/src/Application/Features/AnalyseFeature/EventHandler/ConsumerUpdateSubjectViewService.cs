using System.Threading;
using Algolia.Search.Http;
using Algolia.Search.Models.Search;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedProject.Models;

namespace Application.Features.SubjectFeature.EventHandler;

public class ConsumerAnalyseService : KafkaConsumerAnalyseMethod
{
     public ConsumerAnalyseService(IConfiguration configuration, ILogger<ConsumerAnalyseService> logger, IServiceProvider serviceProvider)
        : base(configuration, logger, serviceProvider, TopicKafkaConstaints.UserAnalyseData, "analyse_consumer_group")
    {

    }

    protected override async Task ProcessMessage(List<AnalyseDataDocumentModel> message, IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        var _sender = serviceProvider.GetRequiredService<ISender>();
        var _logger = serviceProvider.GetRequiredService<ILogger<ConsumerAnalyseService>>();
        var producer = serviceProvider.GetRequiredService<IProducerService>();
        var _dbContext = serviceProvider.GetRequiredService<AnalyseDbContext>();
        if (message is not null)
        {
            List<UserAnalyseEntity> userData = await _dbContext.UserAnalyseEntity
            .Find(Builders<UserAnalyseEntity>.Filter.Empty)
            .ToListAsync();
            if (userData.Any())
            {
                List<RecommendedData> recommendedDatasInsert = new List<RecommendedData>();
                List<RecommendedData> recommendedDatasUpdate = new List<RecommendedData>();
                // Process data
                foreach (UserAnalyseEntity user in userData)
                {
                    if (message is not null)
                    {
                        var topSubjectIds = message
                            .Where(d => d.SubjectId.HasValue && d.UserId == user.UserId)
                            .GroupBy(d => d.SubjectId!.Value)
                            .Select(group => new { SubjectId = group.Key })
                            .Take(4)
                            .Select(x => x.SubjectId)
                            .ToList();
                       

                        var topDocumentIds = message
                            .Where(d => d.DocumentId.HasValue && d.UserId == user.UserId)
                            .GroupBy(d => d.DocumentId!.Value)
                            .Select(group => new { DocumentId = group.Key, Count = group.Count() })
                            .OrderByDescending(x => x.Count)
                            .Take(8)
                            .Select(x => x.DocumentId)
                            .ToList();

                        var topFlashcardIds = message
                            .Where(d => d.FlashcardId.HasValue && d.UserId == user.UserId)
                            .GroupBy(d => d.FlashcardId!.Value)
                            .Select(group => new { FlashcardId = group.Key, Count = group.Count() })
                            .OrderByDescending(x => x.Count)
                            .Take(8)
                            .Select(x => x.FlashcardId)
                            .ToList();
                        if (topSubjectIds.IsNullOrEmpty()
                            && topDocumentIds.IsNullOrEmpty()
                            && topFlashcardIds.IsNullOrEmpty())
                        {
                            continue;
                        }
                        else
                        {

                            var test = await _dbContext.RecommendedData.Find(x => x.UserId == user.UserId).FirstOrDefaultAsync();
                            if (test != null)
                            {
                                var recommendedData = new RecommendedData
                                {
                                    Id = test.Id,
                                    UserId = user.UserId,
                                    SubjectIds = topSubjectIds,
                                    DocumentIds = topDocumentIds,
                                    FlashcardIds = topFlashcardIds,
                                    Grade = user.Grade,
                                    TypeExam = user.TypeExam
                                };
                                recommendedDatasUpdate.Add(recommendedData);
                            }
                            else
                            {
                                // Create the recommended data object
                                var recommendedData = new RecommendedData
                                {
                                    Id = ObjectId.GenerateNewId().ToString(),
                                    UserId = user.UserId,
                                    SubjectIds = topSubjectIds,
                                    DocumentIds = topDocumentIds,
                                    FlashcardIds = topFlashcardIds,
                                    Grade = user.Grade,
                                    TypeExam = user.TypeExam
                                };
                                // Add to the batch list
                                recommendedDatasInsert.Add(recommendedData);
                            }
                        }
                    }
                }
                // Send batch messages to Kafka
                if (recommendedDatasInsert.Any())
                {
                    foreach (var recommendedData in recommendedDatasInsert)
                    {
                        // Send each recommended data in the batch
                        await producer.ProduceObjectWithKeyAsyncBatch(TopicKafkaConstaints.DataRecommended, recommendedData.UserId.ToString(), recommendedData);
                    }
                    await producer.FlushedData(TimeSpan.FromSeconds(10));
                    await _dbContext.RecommendedData.InsertManyAsync(recommendedDatasInsert, cancellationToken: stoppingToken);
                }
                if (recommendedDatasUpdate.Any())
                {
                    foreach (var recommendedData in recommendedDatasUpdate)
                    {
                        // Send each recommended data in the batch
                        var filter = Builders<RecommendedData>.Filter.Eq(n => n.Id, recommendedData.Id);
                        await _dbContext.RecommendedData.FindOneAndReplaceAsync(filter, recommendedData, cancellationToken: stoppingToken);
                        
                    } 
                }
            }
            _logger.LogInformation("Processing complete");
        }
    }
}
//_logger.LogError("sdkjghflksjdfhglkjsdhjfgl;jsdl;kfgjl;ksdjfg;l before" + subject.View);
//                    subject.View += data.Count();
//                    var update = unitOfWork.SubjectRepository.Update(subject);
//var result = await unitOfWork.SaveChangesAsync();
//                    if (result > 0)
//                    {
//                        _logger.LogError($"Update Successfully!!!", subject.Id);
//                    }
//                    _logger.LogError($"Update Fail!!!");
//_logger.LogError("sdkjghflksjdfhglkjsdhjfgl;jsdl;kfgjl;ksdjfg;l after" + subject.View);
