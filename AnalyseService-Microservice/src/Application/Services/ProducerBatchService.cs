using Application.Common.Interfaces.KafkaInterface;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Application.Services;

public class ProducerBatchService : IProducerBatchService
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<ProducerBatchService> _logger;
    private readonly List<Message<string, string>> _batchBuffer;
    private readonly object _lock = new();

    private readonly int _batchSizeLimit = 30;
    private readonly TimeSpan _batchTimeLimit = TimeSpan.FromSeconds(3);
    private DateTime _lastBatchTime = DateTime.UtcNow;

    public ProducerBatchService(IConfiguration configuration, ILogger<ProducerBatchService> logger)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            SaslUsername = configuration["Kafka:SaslUsername"],
            SaslPassword = configuration["Kafka:SaslPassword"],
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            Acks = Acks.All,
            CompressionType = CompressionType.Gzip,
            MessageTimeoutMs = 10000,    // Increase to 10 seconds
            RequestTimeoutMs = 10000,    // Increase to 10 seconds
            RetryBackoffMs = 1000,       // Keep as is
        };
        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _logger = logger;
        _batchBuffer = new List<Message<string, string>>();
    }

    public void QueueMessage<T>(string topic, string key, T obj)
    {
        string json = JsonConvert.SerializeObject(obj);
        var kafkaMessage = new Message<string, string> { Key = key, Value = json };

        lock (_lock)
        {
            _batchBuffer.Add(kafkaMessage);

            if (_batchBuffer.Count >= _batchSizeLimit || (DateTime.UtcNow - _lastBatchTime) >= _batchTimeLimit)
            {
                SendBatch(topic);
            }
        }
    }

    private void SendBatch(string topic)
    {
        var messagesToSend = new List<Message<string, string>>();

        lock (_lock)
        {
            if (_batchBuffer.Count == 0) return;

            messagesToSend.AddRange(_batchBuffer);
            _batchBuffer.Clear();
            _lastBatchTime = DateTime.UtcNow;
        }
        
        try
        {
            foreach (var message in messagesToSend)
            {
                _producer.Produce(topic, message);
            }

            _logger.LogInformation($"Successfully produced a batch of {messagesToSend.Count} messages to topic '{topic}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error producing batch: {ex.Message}");
            // Handle retry or persist failed batch
        }
    }

    public async Task FlushAsync()
    {
        await Task.Run(() => _producer.Flush(TimeSpan.FromSeconds(10)));
    }
}
