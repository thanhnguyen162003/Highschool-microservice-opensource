using Application.Common.Kafka;
using Application.Constants;
using Infrastructure.Data;
using MongoDB.Driver;
using Newtonsoft.Json;
using SharedProject.ConsumeModel.Document;

namespace Application.Services.EventHandler
{
	public class ConsumerSubjectDelete(
        IConfiguration configuration,
        ILogger<ConsumerSubjectDelete> logger,
        IServiceProvider serviceProvider,
        CareerMongoDatabaseContext mongoContext) : KafkaConsumerBase<SubjectDeletedModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.SubjectDeleted, "user_service_group")
	{
        protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			var scopedProvider = scope.ServiceProvider;
			var mongoContext = scopedProvider.GetRequiredService<CareerMongoDatabaseContext>();
			var logger = scopedProvider.GetRequiredService<ILogger<ConsumerSubjectDelete>>();
			try
			{
				// Deserialize message
				var subjectDeleted = JsonConvert.DeserializeObject<SubjectDeletedModel>(message);
				if (subjectDeleted == null)
				{
					logger.LogWarning("Received an invalid SubjectDeletedModel message.");
					return;
				}

				// Delete from MongoDB
				var deleteResult = await mongoContext.SubjectMajors.DeleteOneAsync(s => s.SubjectMasterId == subjectDeleted.SubjectId);

				if (deleteResult.DeletedCount > 0)
				{
					logger.LogInformation($"Deleted SubjectMajor with SubjectMasterId: {subjectDeleted.SubjectId}");
				}
				else
				{
					logger.LogWarning($"No SubjectMajor found with SubjectMasterId: {subjectDeleted.SubjectId}");
				}
			}
			catch (Exception ex)
			{
				logger.LogError($"Error processing SubjectDeletedModel: {ex.Message}");
			}
		}
	}
}
