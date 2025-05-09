using Application.Common.Interfaces.KafkaInterface;
using Application.Constants;
using Confluent.Kafka;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System.Diagnostics;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Application.Common.Kafka;

public abstract class KafkaConsumerFlashcardViewMethod : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerFlashcardViewMethod> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _topicName;
    public KafkaConsumerFlashcardViewMethod(IConfiguration configuration, ILogger<KafkaConsumerFlashcardViewMethod> logger,
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
            var messageCounts = new Dictionary<string, int>();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                while (!stoppingToken.IsCancellationRequested)
                {
                    // Check if 5 minutes have elapsed
                   if (stopwatch.ElapsedMilliseconds >= 1000 * 60 * 5)
                    {
                        stopwatch = Stopwatch.StartNew();
                        await ProcessMessage(messageCounts, scope.ServiceProvider);

                        messageCounts.Clear();

                    }


                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(1000));
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
        });
    }
    protected abstract Task ProcessMessage(Dictionary<string, int> message, IServiceProvider serviceProvider);
}

//Dictionary<string, int> data = await ConsumeByKeyAsync(topic, subjects, stoppingToken);
//if (data is not null)
//{
//    foreach (var item in data)
//    {
//        if (item.Value > 0)
//        {
//            Guid subjectId = Guid.Parse(item.Key);
//            var subject = await unitOfWork.SubjectRepository.Get(filter: x => x.Id == subjectId);
//            var subjectUpdateData = _mapper.Map<Subject>(subject.FirstOrDefault());
//            subjectUpdateData.View += item.Value;
//            var result = await unitOfWork.SubjectRepository.UpdateSubject(subjectUpdateData, stoppingToken);
//            if (result is false)
//            {
//                _logger.LogError("Update Fail" + subjectUpdateData.Id);
//            }
//        }
//    }
//}