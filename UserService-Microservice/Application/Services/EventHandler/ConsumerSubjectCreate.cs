using Application.Common.Kafka;
using SharedProject.ConsumeModel.Document;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using Application.Constants;
using Domain.MongoEntities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.EventHandler
{
	public class ConsumerSubjectCreate(
        IConfiguration configuration,
        ILogger<ConsumerSubjectCreate> logger,
        IServiceProvider serviceProvider) : KafkaConsumerBaseBatch<SubjectCreatedModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.SubjectCreated, "user_service_group",
				  new BatchProcessingOptions
				  {
					  BatchSize = 20,                         // Process up to 20 creates at once
					  BatchTimeout = TimeSpan.FromSeconds(5),  // Process batch after 5 seconds if not full
					  MaxConcurrentBatches = 2                 // Allow up to 2 concurrent batches
				  })
	{
		private readonly ILogger<ConsumerSubjectCreate> _logger = logger;

        protected override async Task ProcessBatch(IEnumerable<string> messages, IServiceProvider serviceProvider)
		{
			var mongoContext = serviceProvider.GetRequiredService<CareerMongoDatabaseContext>();

			// Track metrics for this batch
			int insertCount = 0;
			int updateCount = 0;
			int errorCount = 0;
			int invalidMessageCount = 0;

			// Group operations by type to potentially optimize database access
			var allSubjectCreatedModels = new List<SubjectCreatedModel>();

			// First pass: deserialize all messages
			foreach (var message in messages)
			{
				try
				{
					var subjectCreated = JsonConvert.DeserializeObject<SubjectCreatedModel>(message);

					if (subjectCreated == null)
					{
						_logger.LogWarning("Received an invalid SubjectCreatedModel message.");
						invalidMessageCount++;
						continue;
					}

					allSubjectCreatedModels.Add(subjectCreated);
				}
				catch (JsonException ex)
				{
					_logger.LogError($"Failed to deserialize message: {ex.Message}");
					errorCount++;
				}
				catch (Exception ex)
				{
					_logger.LogError($"Error processing message: {ex.Message}");
					errorCount++;
				}
			}

			// If we have valid models to process
			if (allSubjectCreatedModels.Count > 0)
			{
				var collection = mongoContext.SubjectMajors;

				// Get all subject IDs to check
				var subjectIds = allSubjectCreatedModels.Select(s => s.SubjectId).ToList();

				// Find all existing subjects in one query
				var filter = Builders<SubjectMajor>.Filter.In(s => s.SubjectMasterId, subjectIds);
				var existingSubjects = await collection.Find(filter)
					.ToListAsync();

				// Create a lookup for faster access
				var existingSubjectsDict = existingSubjects.ToDictionary(s => s.SubjectMasterId);

				// Process each model - updates and inserts
				var bulkWrites = new List<WriteModel<SubjectMajor>>();

				foreach (var model in allSubjectCreatedModels)
				{
					try
					{
						if (existingSubjectsDict.ContainsKey(model.SubjectId))
						{
							// Update existing document
							var updateDefinition = Builders<SubjectMajor>.Update
								.Set(s => s.SubjectMasterName, model.MasterSubjectName ?? "Unknown")
								.Set(s => s.SubjectMasterSlug, model.MasterSubjectSlug);

							var updateModel = new UpdateOneModel<SubjectMajor>(
								Builders<SubjectMajor>.Filter.Eq(s => s.SubjectMasterId, model.SubjectId),
								updateDefinition);

							bulkWrites.Add(updateModel);
							updateCount++;
						}
						else
						{
							// Insert new document
							var newSubject = new SubjectMajor
							{
								Id = ObjectId.GenerateNewId().ToString(),
								SubjectMasterName = model.MasterSubjectName ?? "Unknown",
								SubjectMasterId = model.SubjectId,
								SubjectMasterSlug = model.MasterSubjectSlug
							};

							var insertModel = new InsertOneModel<SubjectMajor>(newSubject);
							bulkWrites.Add(insertModel);
							insertCount++;
						}
					}
					catch (Exception ex)
					{
						_logger.LogError($"Error preparing operation for SubjectId {model.SubjectId}: {ex.Message}");
						errorCount++;
					}
				}

				// Execute all operations in one bulk write if we have any
				if (bulkWrites.Count > 0)
				{
					try
					{
						var bulkWriteResult = await collection.BulkWriteAsync(bulkWrites);
						_logger.LogInformation($"Bulk write completed. Matched: {bulkWriteResult.MatchedCount}, " +
											  $"Modified: {bulkWriteResult.ModifiedCount}, " +
											  $"Inserted: {bulkWriteResult.InsertedCount}");
					}
					catch (Exception ex)
					{
						_logger.LogError($"Error executing bulk write: {ex.Message}");
						errorCount += bulkWrites.Count; // All operations failed
						insertCount = 0;
						updateCount = 0;
					}
				}
			}

			// Log batch summary
			_logger.LogInformation(
				$"Batch processed: {insertCount} inserted, {updateCount} updated, " +
				$"{invalidMessageCount} invalid messages, {errorCount} errors out of {messages.Count()} total messages");
		}
	}
}