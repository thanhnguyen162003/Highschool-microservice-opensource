using Confluent.Kafka;

namespace Application.Common.Kafka;

public abstract class KafkaConsumerBase5Minutes<T> : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KafkaConsumerBase5Minutes<T>> _logger;
        private readonly string _topicName;

        protected KafkaConsumerBase5Minutes(IConfiguration configuration, ILogger<KafkaConsumerBase5Minutes<T>> logger, IServiceProvider serviceProvider, string topicName, string groupId)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _topicName = topicName;

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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topicName);
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                try
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(5));
                    if (consumeResult != null)
                    {
                        await ProcessMessage(consumeResult.Message.Value, scope.ServiceProvider);
                    }
                    else
                    {
                        _logger.LogInformation("No messages available.");
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

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _consumer.Close();
        }

        protected abstract Task ProcessMessage(string message, IServiceProvider serviceProvider);
    }