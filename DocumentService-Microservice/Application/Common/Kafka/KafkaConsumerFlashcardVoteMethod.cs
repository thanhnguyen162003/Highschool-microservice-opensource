using Application.Common.Interfaces.KafkaInterface;
using Application.Constants;
using Confluent.Kafka;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System.Diagnostics;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Application.Common.Kafka;

public abstract class KafkaConsumerFlashcardVoteMethod : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerFlashcardVoteMethod> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _topicName;
    public KafkaConsumerFlashcardVoteMethod(IConfiguration configuration, ILogger<KafkaConsumerFlashcardVoteMethod> logger,
        IServiceProvider serviceProvider, string topicName, string groupId)
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
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {
            var scope = _serviceProvider.CreateScope();

            _consumer.Subscribe(_topicName);
            var messageVoting = new Dictionary<string, (double sum , int count)>();
            try
            {
                var stopwatch = Stopwatch.StartNew();

                while (!stoppingToken.IsCancellationRequested)
                {
                    // Check if 5 minutes have elapsed
                   if (stopwatch.ElapsedMilliseconds >= 1000 * 60 * 5)
                    {
                        stopwatch = Stopwatch.StartNew();
                        await ProcessMessage(messageVoting, scope.ServiceProvider);

                        messageVoting = new Dictionary<string, (double sum, int count)>();
                    }


                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(1000));
                    if (consumeResult != null)
                    {

                        if (messageVoting.ContainsKey(consumeResult.Message.Key))
                        {
                            // Update the sum and count for the existing key
                            var (sum, count) = messageVoting[consumeResult.Message.Key];
                            messageVoting[consumeResult.Message.Key] = (sum + double.Parse(consumeResult.Message.Value), count + 1);
                        }
                        else
                        {
                            messageVoting[consumeResult.Message.Key] = (double.Parse(consumeResult.Message.Value), 1);
                        }

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

            finally
            {
                // Ensure consumer is closed to free resources
                _consumer.Close();
                _consumer.Dispose();
            }
        });
    }
    protected abstract Task ProcessMessage(Dictionary<string, (double sum, int count)> message, IServiceProvider serviceProvider);
}
