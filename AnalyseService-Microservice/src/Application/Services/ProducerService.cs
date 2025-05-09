using Application.Common.Interfaces.KafkaInterface;
using Confluent.Kafka;
using MongoDB.Driver;
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
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            SaslUsername = configuration["Kafka:SaslUsername"],
            SaslPassword = configuration["Kafka:SaslPassword"],
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            Acks = Acks.All,
            MessageSendMaxRetries = 3,
            CompressionType = CompressionType.Gzip,
            LingerMs = 2000,
            // Adjust timeout configurations
            MessageTimeoutMs = 10000,    // Increase to 10 seconds
            RequestTimeoutMs = 10000,    // Increase to 10 seconds
            RetryBackoffMs = 1000,       // Keep as is
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
    public Task ProduceObjectWithKeyAsyncBatch<T>(string topic, string key, T obj)
    {
        try
        {
            string json = JsonConvert.SerializeObject(obj);
            var kafkaMessage = new Message<string, string> { Key = key, Value = json };
            _producer.ProduceAsync(topic, kafkaMessage);
        }
        catch (ProduceException<string, string> ex)
        { 
            _logger.LogError($"Failed to deliver message: {ex.Error.Reason}");
            return Task.FromException(ex);
        }
        return Task.CompletedTask;
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
    
    // public async Task<bool> ProduceWithRetryAsync(string topic, string key, string message, int maxRetries = 3)
    // {
    //     int attempt = 0;
    //     while (attempt < maxRetries)
    //     {
    //         try
    //         {
    //             var kafkaMessage = new Message<string, string> { Key = key, Value = message };
    //             var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);
    //             Console.WriteLine($"Produced message to '{deliveryResult.TopicPartitionOffset}' with key: {key}");
    //             return true;
    //         }
    //         catch (ProduceException<string, string> ex)
    //         {
    //             _logger.LogError($"Attempt {attempt + 1} failed to deliver message with key: {key}. Error: {ex.Error.Reason}");
    //             attempt++;
    //         }
    //     }
    //
    //     // If max retries are reached, log and persist the failed message
    //     _logger.LogError($"Failed to deliver message with key: {key} after {maxRetries} attempts.");
    //     // Optionally, store the message to a database or file for later reprocessing
    //     SaveToPersistentStorage(topic, key, message);
    //     return false;
    // }

    
    public async Task FlushedData(TimeSpan timeSpan)
    {
        _producer.Flush(timeSpan);
    }
}   
