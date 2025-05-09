using Confluent.Kafka;
using System.Collections.Concurrent;

namespace Application.Common.Kafka
{
    public abstract class KafkaConsumerBaseBatch3Minus<T> : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KafkaConsumerBaseBatch3Minus<T>> _logger;
        private readonly List<string> _topicName;

        // Configuration for batch processing
        private readonly int _batchSize = 20;
        private readonly TimeSpan _batchTimeout = TimeSpan.FromSeconds(3); 
        private readonly ConcurrentBag<string> _messageBatch = new();

        protected KafkaConsumerBaseBatch3Minus(IConfiguration configuration, ILogger<KafkaConsumerBaseBatch3Minus<T>> logger,
            IServiceProvider serviceProvider, List<string> topicName, string groupId)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _topicName = topicName ?? throw new ArgumentNullException(nameof(topicName));

            var consumerConfig = new ConsumerConfig
            {
				BootstrapServers = configuration["Kafka:BootstrapServers"],
				SaslUsername = configuration["Kafka:SaslUsername"],
				SaslPassword = configuration["Kafka:SaslPassword"],
				SecurityProtocol = SecurityProtocol.SaslSsl,
				SaslMechanism = SaslMechanism.Plain,
				GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
            };

            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        }

        protected KafkaConsumerBaseBatch3Minus(IConfiguration configuration, ILogger<KafkaConsumerBaseBatch3Minus<T>> logger, 
            IServiceProvider serviceProvider, string topicName, string groupId)
            : this(configuration, logger, serviceProvider, new List<string> { topicName }, groupId)
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topicName);
            var lastBatchTime = DateTime.UtcNow;

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                try
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));
                    if (consumeResult != null)
                    {
                        _messageBatch.Add(consumeResult.Message.Value);

                        // Check if batch conditions are met
                        if (_messageBatch.Count >= _batchSize || 
                            DateTime.UtcNow - lastBatchTime >= _batchTimeout)
                        {
                            var messagesToProcess = _messageBatch.ToArray(); // Snapshot the batch
                            _messageBatch.Clear(); // Clear for the next batch
                            lastBatchTime = DateTime.UtcNow;

                            // Process the batch asynchronously
                            Task.Run(() => ProcessBatch(messagesToProcess, scope.ServiceProvider));
                        }
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"Consume error: {e.Error.Reason}");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Kafka consumption was canceled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing Kafka message: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
            _consumer.Unsubscribe();
            _consumer.Close();
            _logger.LogInformation("Kafka consumer closed.");
        }

        /// <summary>
        /// Abstract method to process a batch of messages.
        /// </summary>
        /// <param name="messages">Batch of messages</param>
        /// <param name="serviceProvider">Service provider for scoped dependencies</param>
        /// <returns></returns>
        protected abstract Task ProcessBatch(IEnumerable<string> messages, IServiceProvider serviceProvider);
    }
}
