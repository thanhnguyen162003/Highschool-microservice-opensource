using Application.Features.NewsFeature.Queries;
using Confluent.Kafka;
using Domain.Entities.SqlEntites;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Driver;
using System.Diagnostics;
using System.Linq;

namespace Application.Common.Kafka;

public abstract class KafkaConsumerNewsViewMethod : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerNewsViewMethod> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _topicName;
    
    public KafkaConsumerNewsViewMethod(IConfiguration configuration, ILogger<KafkaConsumerNewsViewMethod> logger,
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
            var unitOfWork = scope.ServiceProvider.GetRequiredService<MediaDbContext>();
            var lastClearTrendingTime = DateTime.UtcNow;
            _consumer.Subscribe(_topicName);


            var messageCounts = new Dictionary<string, int>();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                while (!stoppingToken.IsCancellationRequested)
                {
                    // Check if 30 minutes have elapsed
                    if (stopwatch.ElapsedMilliseconds >= 1000 * 60 * 30)
                    {
                        stopwatch = Stopwatch.StartNew();
                        await ProcessMessage(messageCounts, scope.ServiceProvider);

                        messageCounts.Clear();

                    }


                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(500));
                    if (consumeResult != null)
                    {
                        if (messageCounts.ContainsKey(consumeResult.Message.Key))
                        {
                            // Increment the count for the existing key
                            messageCounts[consumeResult.Message.Key]++;
                        }
                        else
                        {
                            messageCounts[consumeResult.Message.Key] = 1;
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
        }
        );
    }

    protected abstract Task ProcessMessage(Dictionary<string, int> message, IServiceProvider serviceProvider);
}

