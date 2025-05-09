using Confluent.Kafka;
using Domain.Common.Interfaces.KafkaInterface;
using Newtonsoft.Json;

namespace Domain.Services;
public class ProducerService : IProducerService
{
	private readonly IConfiguration _configuration;
	private readonly IProducer<string, string> _producer;

	public ProducerService(IConfiguration configuration)
	{
		_configuration = configuration;

		var producerConfig = new ProducerConfig
		{
			BootstrapServers = configuration["Kafka:BootstrapServers"],
			SaslUsername = configuration["Kafka:SaslUsername"],
			SaslPassword = configuration["Kafka:SaslPassword"],
			SecurityProtocol = SecurityProtocol.SaslSsl,
			SaslMechanism = SaslMechanism.Plain,
			Acks = Acks.All,
			CompressionType = CompressionType.Gzip
		};

		// Create the Kafka producer with string keys and values
		_producer = new ProducerBuilder<string, string>(producerConfig).Build();
	}

	public IEnumerable<string> GetTopics()
	{
		var config = new AdminClientConfig
		{

			BootstrapServers = _configuration["Kafka:BootstrapServers"],
            SaslUsername = _configuration["Kafka:SaslUsername"],
            SaslPassword = _configuration["Kafka:SaslPassword"],
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            Acks = Acks.All
		};

		using (var adminClient = new AdminClientBuilder(config).Build())
		{
			try
			{
				var metadata = adminClient.GetMetadata(TimeSpan.FromDays(10));
				var topics = new List<string>();

				foreach (var topic in metadata.Topics)
				{
					topics.Add(topic.Topic);
				}

				return topics;
			} catch (KafkaException ex)
			{
				Console.WriteLine($"Error retrieving Kafka topics: {ex.Message}");
				throw;
			}
		}
	}

	public async Task ProduceAsync(string topic, string message)
	{
		try
		{
			var kafkaMessage = new Message<string, string> { Value = message };

			var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);

			Console.WriteLine($"Produced message to '{deliveryResult.TopicPartitionOffset}' with value: {message}");
		} catch (ProduceException<string, string> ex)
		{
			Console.WriteLine($"Failed to deliver message: {ex.Error.Reason}");
		}
	}

	public async Task ProduceWithKeyAsync(string topic, string key, string message)
	{
		try
		{
			var kafkaMessage = new Message<string, string> { Key = key, Value = message };

			var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);

			Console.WriteLine($"Produced message to '{deliveryResult.TopicPartitionOffset}' with key: {key} and value: {message}");
		} catch (ProduceException<string, string> ex)
		{
			Console.WriteLine($"Failed to deliver message with key: {key}. Error: {ex.Error.Reason}");
		}
	}

	public async Task<bool> ProduceObjectWithKeyAsync<T>(string topic, string key, T obj)
	{
		try
		{
			string json = JsonConvert.SerializeObject(obj);
			var kafkaMessage = new Message<string, string> { Key = key, Value = json };

			var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);

			Console.WriteLine($"Produced message to '{deliveryResult.TopicPartitionOffset}' with value: {json}");

			return true;
		} catch (ProduceException<string, string> ex)
		{
			Console.WriteLine($"Failed to deliver message: {ex.Error.Reason}");

			return false;
		}
	}

	public async Task<bool> ProduceObjectAsync<T>(string topic, T obj)
	{
		try
		{
			string json = JsonConvert.SerializeObject(obj);
			var kafkaMessage = new Message<string, string> { Value = json };

			var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);

			Console.WriteLine($"Produced message to '{deliveryResult.TopicPartitionOffset}' with value: {json}");

			return true;
		} catch (ProduceException<string, string> ex)
		{
			Console.WriteLine($"Failed to deliver message: {ex.Error.Reason}");

			return false;
		}
	}
}