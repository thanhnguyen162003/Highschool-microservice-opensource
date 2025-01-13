using Confluent.Kafka;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;
using SharedProject.Models;
using System.Diagnostics;

namespace Application.Common.Kafka;

public abstract class KafkaConsumerAnalyseMethod : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerAnalyseMethod> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _topicName;
    public KafkaConsumerAnalyseMethod(IConfiguration configuration, ILogger<KafkaConsumerAnalyseMethod> logger,
        IServiceProvider serviceProvider, string topicName, string groupId)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _topicName = topicName;
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            // SaslUsername = configuration["Kafka:SaslUsername"],
            // SaslPassword = configuration["Kafka:SaslPassword"],
            // SecurityProtocol = SecurityProtocol.SaslSsl,
            // SaslMechanism = SaslMechanism.Plain,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope();
        
        _consumer.Subscribe(_topicName);
        List<AnalyseDataDocumentModel> data = new List<AnalyseDataDocumentModel>();
        try
            {
                var stopwatch = Stopwatch.StartNew();

                while (!stoppingToken.IsCancellationRequested)
                {
                // Check if 30 minutes have elapsed
                    var nowUtc7 = DateTimeOffset.Now.UtcDateTime.AddHours(7);
                    if (nowUtc7.Hour == 0 && nowUtc7.Minute < 5)
                    {
                        data.Clear();
                        await ProcessMessage(data, scope.ServiceProvider,stoppingToken);
                    }
                    var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(2));
                    if (consumeResult == null)
                    {
                        return;
                    }

                    
                    if (!consumeResult.Message.Key.IsNullOrEmpty())
                    {
                        data.Add(JsonConvert.DeserializeObject<AnalyseDataDocumentModel>(consumeResult.Message.Value));
                    }
                    else
                    {
                        // Skip messages with unexpected keys
                        Console.WriteLine($"Skipping message with unexpected key: {consumeResult.Message.Key}");
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
    protected abstract Task ProcessMessage(List<AnalyseDataDocumentModel> message, IServiceProvider serviceProvider, CancellationToken stoppingToken);
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
