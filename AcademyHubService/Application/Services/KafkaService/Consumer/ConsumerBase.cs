using Application.Common.Messages;
using Confluent.Kafka;
using Domain.Models.Settings;
using Microsoft.Extensions.Options;

namespace Application.Services.KafkaService.Consumer
{
    /// <summary>
    /// Base class for a Kafka consumer service that processes messages in batches.
    /// This class is responsible for consuming messages from a Kafka topic and processing them in batches
    /// using multiple worker tasks. The class handles the consumer lifecycle, including subscribing,
    /// consuming messages, and shutting down gracefully.
    /// </summary>
    /// <typeparam name="TDomain">The type of domain object being processed (used for abstraction).</typeparam>
    public abstract class ConsumerBase<TDomain> : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly BatchProcessor _batchProcessor;

        protected readonly IServiceScopeFactory _scopeFactory;
        protected readonly ILogger<ConsumerBase<TDomain>> _logger;

        private readonly KafkaSetting _kafkaSetting;
        private readonly ConsumeSetting _consumeSetting;

        /// <summary>
        /// Initializes the Kafka consumer and the batch processor.
        /// </summary>
        /// <param name="serviceProvider">The service provider to create scoped services.</param>
        /// <param name="logger">The logger for logging events.</param>
        /// <param name="options">Kafka settings.</param>
        /// <param name="topicName">The Kafka topic to subscribe to.</param>
        /// <param name="workerCount">The number of worker tasks to process batches (default is 3).</param>
        protected ConsumerBase(IServiceScopeFactory serviceProvider, ILogger<ConsumerBase<TDomain>> logger, IOptions<KafkaSetting> options, Action<ConsumeSetting> config)
        {
            _scopeFactory = serviceProvider;
            _logger = logger;
            _kafkaSetting = options.Value;
            _consumeSetting = new ConsumeSetting();
            config(_consumeSetting);

            // Initialize the batch processor with a custom number of workers
            _batchProcessor = new BatchProcessor(logger);

            // Configure the Kafka consumer
            var optionConsumer = new ConsumerConfig()
            {
                BootstrapServers = _kafkaSetting.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
            };

            // Configure authentication if enabled
            if (_kafkaSetting.IsAuthentication)
            {
                optionConsumer.SaslUsername = _kafkaSetting.SaslUsername;
                optionConsumer.SaslPassword = _kafkaSetting.SaslPassword;
                optionConsumer.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                optionConsumer.SaslMechanism = SaslMechanism.Plain;
            }

            // Create the Kafka consumer with string keys and values
            _consumer = new ConsumerBuilder<string, string>(optionConsumer).Build();
        }

        /// <summary>
        /// The main loop for the Kafka consumer. This method is responsible for subscribing to the topic,
        /// consuming messages, and delegating processing to the batch processor. It also handles the worker tasks
        /// and stops the consumer gracefully when cancellation is requested.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token to stop the background service.</param>
        /// <returns>A task representing the ongoing background operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_consumeSetting.TopicName);

            // Create worker tasks to process batches concurrently
            var workerTasks = Enumerable.Range(0, _consumeSetting.WorkerCount)
                .Select(_ => Task.Run(() => _batchProcessor.ProcessBatchWorker(stoppingToken, ProcessBatch), stoppingToken))
                .ToList();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Consume a message from the Kafka topic with a timeout of 100ms
                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(_consumeSetting.TimeoutMessageKafka));

                    if (consumeResult != null)
                    {
                        // Process the incoming message and add it to the batch processor
                        await _batchProcessor.ProcessIncomingMessage(consumeResult.Message.Value, stoppingToken);
                    }
                } catch (ConsumeException e)
                {
                    _logger.LogError($"Consume error: {e.Error.Reason}");
                } catch (OperationCanceledException)
                {
                    _logger.LogInformation("Kafka consumption was canceled.");
                    break;
                } catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error: {ex.Message}");
                }

                // Wait for a short period before trying to consume again
                await Task.Delay(_consumeSetting.TimeConsumeAgain, stoppingToken);
            }

            // Cleanup: Unsubscribe from the Kafka topic and close the consumer
            _consumer.Unsubscribe();
            _consumer.Close();
            _logger.LogInformation(MessageKafka.ConsumerClose);

            // Complete the batch processor
            _batchProcessor.CompleteBatchChannel();

            // Wait for all worker tasks to finish processing
            await Task.WhenAll(workerTasks);
        }

        /// <summary>
        /// Abstract method to process the batch of messages. This method must be implemented in derived classes
        /// to define how the consumed messages are processed in batch.
        /// </summary>
        /// <param name="messages">The batch of messages to process.</param>
        /// <returns>A task representing the processing of the batch.</returns>
        protected abstract Task ProcessBatch(IEnumerable<string> messages);
    }


}
