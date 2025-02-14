using Application.Common.Interfaces.KafkaInterface;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Application.Services;

public class ProducerService : IProducerService
{
    private readonly IConfiguration _configuration;
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<ProducerService> _logger;

    public ProducerService(IConfiguration configuration, ILogger<ProducerService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            SaslUsername = _configuration["Kafka:SaslUsername"],
            SaslPassword = _configuration["Kafka:SaslPassword"],
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            MessageSendMaxRetries = 3,
            Acks = Acks.All,
            CompressionType = CompressionType.Gzip,
            // Timeout configurations
            MessageTimeoutMs = 5000,   // 5 seconds timeout for a single message send
            RequestTimeoutMs = 3000,   // 3 seconds request timeout
            RetryBackoffMs = 100,      // 100 ms backoff between retries
        };
        // Create the Kafka producer with string keys and values
        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }
    
    public async Task<bool> ProduceAsync(string topic, string message)
    {
        try
        {
            var kafkaMessage = new Message<string, string> { Value = message };
            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);
            Console.WriteLine($"Produced message to '{deliveryResult.TopicPartitionOffset}'");
            return true;
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError($"Failed to deliver message: {ex.Error.Reason}");
            return false;
        }
    }

    public async Task<bool> ProduceWithKeyAsync(string topic, string key, string message)
    {
        try
        {
            var kafkaMessage = new Message<string, string> { Key = key, Value = message };
            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);
            Console.WriteLine($"Produced message to '{deliveryResult.TopicPartitionOffset}' with key: {key}");
            return true;
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError($"Failed to deliver message with key: {key}. Error: {ex.Error.Reason}");
            return false;
        }
    }

    public async Task<bool> ProduceObjectWithKeyAsync<T>(string topic, string key, T obj)
    {
        try
        {
            string json = JsonConvert.SerializeObject(obj);
            var kafkaMessage = new Message<string, string> { Key = key, Value = json };
            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);
            Console.WriteLine($"Produced message to '{deliveryResult.TopicPartitionOffset}'");
            return true;
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError($"Failed to deliver message: {ex.Error.Reason}");
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
            return true;
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError($"Failed to deliver message: {ex.Error.Reason}");
            return false;
        }
    }
}   