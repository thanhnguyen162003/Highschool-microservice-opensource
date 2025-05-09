using Confluent.Kafka;
using Domain.Models.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Application.Services.KafkaService.Producer
{
    public class ProducerService : IProducerService
    {
        private readonly IProducer<string, string> _producer;
        private readonly KafkaSetting _kafkaSetting;

        public ProducerService(IOptions<KafkaSetting> options)
        {
            _kafkaSetting = options.Value;

            // Config for producer
            var optionProducer = new ProducerConfig()
            {
                BootstrapServers = _kafkaSetting.BootstrapServers,
                Acks = Acks.All,
                CompressionType = CompressionType.Gzip,
                MessageSendMaxRetries = _kafkaSetting.MessageSendMaxRetries,
                MessageTimeoutMs = _kafkaSetting.MessageTimeoutMs,
                RequestTimeoutMs = _kafkaSetting.RequestTimeoutMs,
                RetryBackoffMs = _kafkaSetting.RetryBackoffMs
            };

            if (_kafkaSetting.IsAuthentication)
            {
                optionProducer.SaslUsername = _kafkaSetting.SaslUsername;
                optionProducer.SaslPassword = _kafkaSetting.SaslPassword;
                optionProducer.SecurityProtocol = SecurityProtocol.SaslSsl;
                optionProducer.SaslMechanism = SaslMechanism.Plain;
            }

            // Create the Kafka producer with string keys and values
            _producer = new ProducerBuilder<string, string>(optionProducer).Build();
        }

        public IEnumerable<string> GetTopics()
        {
            var config = new AdminClientConfig
            {

                BootstrapServers = _kafkaSetting.BootstrapServers,
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

        public async Task<bool> ProduceAsync(string topic, string message)
        {
            try
            {
                var kafkaMessage = new Message<string, string> { Value = message };

                var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);

                Console.WriteLine($"Produced message to '{deliveryResult.TopicPartitionOffset}' with value: {message}");

                return true;
            } catch (ProduceException<string, string> ex)
            {
                Console.WriteLine($"Failed to deliver message: {ex.Error.Reason}");

                return false;
            }
        }

        public async Task<bool> ProduceWithKeyAsync(string topic, string key, string message)
        {
            try
            {
                var kafkaMessage = new Message<string, string> { Key = key, Value = message };

                var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);

                Console.WriteLine($"Produced message to '{deliveryResult.TopicPartitionOffset}' with key: {key} and value: {message}");

                return true;
            } catch (ProduceException<string, string> ex)
            {
                Console.WriteLine($"Failed to deliver message with key: {key}. Error: {ex.Error.Reason}");

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
}
