using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Infrastructure.Contexts;
using Application.Constants;

namespace Application.WorkerService
{
    public class FlashcardDueNotification
    {
        public Guid UserId { get; set; }
        public Guid FlashcardId { get; set; }
        public string FlashcardName { get; set; } = null!;
        public string FlashcardSlug { get; set; } = null!;
        public int DueContentCount { get; set; }
    }
    public class FlashcardNotificationOptions
    {
        public int BatchSize { get; set; } = 100;
        public int CheckIntervalMinutes { get; set; } = 60;
        public int MaxRetries { get; set; } = 3;
        public int RetryBackoffMs { get; set; } = 1000;
    }

    public class FlashcardNotificationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IProducer<Null, string> _kafkaProducer;
        private readonly ILogger<FlashcardNotificationWorker> _logger;
        private readonly FlashcardNotificationOptions _options;
        private readonly TimeSpan _checkInterval;

        public FlashcardNotificationWorker(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            ILogger<FlashcardNotificationWorker> logger,
            IOptions<FlashcardNotificationOptions> options)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _checkInterval = TimeSpan.FromMinutes(_options.CheckIntervalMinutes);

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                SaslUsername = configuration["Kafka:SaslUsername"],
                SaslPassword = configuration["Kafka:SaslPassword"],
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                EnableIdempotence = true,
                Acks = Acks.All,
                MessageSendMaxRetries = _options.MaxRetries,
                RetryBackoffMs = _options.RetryBackoffMs,
                CompressionType = CompressionType.Snappy,
            };

            _kafkaProducer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FlashcardNotificationWorker started with check interval of {Interval} minutes", _options.CheckIntervalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessDueFlashcardsAsync(DateTime.UtcNow, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("FlashcardNotificationWorker operation canceled");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing due flashcards");
                }

                var nextRunTime = CalculateNextRunTime(DateTime.UtcNow);
                var delayTime = nextRunTime - DateTime.UtcNow;

                if (delayTime > TimeSpan.Zero)
                {
                    _logger.LogInformation("Next check scheduled for {NextRunTime}", nextRunTime);
                    await Task.Delay(delayTime, stoppingToken);
                }
                else
                {
                    await Task.Delay(_checkInterval, stoppingToken);
                }
            }
        }

        private DateTime CalculateNextRunTime(DateTime currentTime)
        {
            return currentTime.AddMinutes(_options.CheckIntervalMinutes)
                             .AddSeconds(-currentTime.Second)
                             .AddMilliseconds(-currentTime.Millisecond);
        }

        private async Task ProcessDueFlashcardsAsync(DateTime currentDate, CancellationToken cancellationToken)
        {
            var processingStartTime = DateTime.UtcNow;
            var dueWindowStart = currentDate.AddHours(8); 
            var dueWindowEnd = currentDate.AddHours(9); 
            _logger.LogInformation("Checking for due flashcards at {Time}", currentDate);

            int batchSize = _options.BatchSize;
            int skip = 0;
            int totalProcessed = 0;
            bool hasMoreRecords;
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DocumentDbContext>();

            do
            {
                var queryStartTime = DateTime.UtcNow;
                List<FlashcardDueNotification> dueNotifications;

                try
                {
                    dueNotifications = await dbContext.UserFlashcardProgresses
                        .AsNoTracking()
                        .Where(ufp => ufp.DueDate.HasValue &&
                                      ufp.DueDate >= dueWindowStart &&
                                      ufp.DueDate < dueWindowEnd &&
                                      (ufp.LastReviewDate == null || ufp.LastReviewDate < ufp.DueDate))
                        .Select(ufp => new
                        {
                            ufp.UserId,
                            ufp.FlashcardContentId
                        })
                        .Join(
                            dbContext.FlashcardContents.AsNoTracking().Select(fc => new
                            {
                                fc.Id,
                                fc.FlashcardId
                            }),
                            ufp => ufp.FlashcardContentId,
                            fc => fc.Id,
                            (ufp, fc) => new { UserId = ufp.UserId, FlashcardId = fc.FlashcardId }
                        )
                        .Join(
                            dbContext.Flashcards.AsNoTracking().Select(f => new
                            {
                                f.Id,
                                f.FlashcardName,
                                f.Slug
                            }),
                            x => x.FlashcardId,
                            f => f.Id,
                            (x, f) => new { x.UserId, FlashcardId = f.Id, f.FlashcardName, f.Slug }
                        )
                        .GroupBy(x => new { x.UserId, x.FlashcardId, x.FlashcardName, x.Slug })
                        .Select(g => new FlashcardDueNotification
                        {
                            UserId = g.Key.UserId,
                            FlashcardId = g.Key.FlashcardId,
                            FlashcardSlug = g.Key.Slug,
                            FlashcardName = g.Key.FlashcardName,
                            DueContentCount = g.Count()
                        })
                        .OrderBy(n => n.UserId)
                        .ThenBy(n => n.FlashcardId)
                        .Skip(skip)
                        .Take(batchSize)
                        .ToListAsync(cancellationToken);

                    var queryDuration = DateTime.UtcNow - queryStartTime;
                    _logger.LogInformation("Query executed in {Duration}ms, retrieved {Count} notifications",
                        queryDuration.TotalMilliseconds, dueNotifications.Count);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Database query error while retrieving due flashcards at skip {Skip}", skip);
                    throw;
                }

                hasMoreRecords = dueNotifications.Count > 0;
                skip += batchSize;
                totalProcessed += dueNotifications.Count;

                if (dueNotifications.Count > 0)
                {
                    await PublishNotificationsAsync(dueNotifications, cancellationToken);
                }

            } while (hasMoreRecords && !cancellationToken.IsCancellationRequested);

            var processingDuration = DateTime.UtcNow - processingStartTime;
            _logger.LogInformation("Finished processing {TotalProcessed} due flashcards in {Duration}ms",
                totalProcessed, processingDuration.TotalMilliseconds);
        }

        private async Task PublishNotificationsAsync(List<FlashcardDueNotification> notifications, CancellationToken cancellationToken)
        {
            var publishStartTime = DateTime.UtcNow;
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            const int publishBatchSize = 100;
            var batches = notifications
                .Select((notification, index) => new { notification, index })
                .GroupBy(x => x.index / publishBatchSize)
                .Select(g => g.Select(x => x.notification).ToList())
                .ToList();

            foreach (var batch in batches)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var publishTasks = batch.Select(async notification =>
                {
                    try
                    {
                        var message = JsonSerializer.Serialize(notification, serializerOptions);

                        var deliveryResult = await _kafkaProducer.ProduceAsync(
                            TopicKafkaConstaints.FlashcardDueNotification,
                            new Message<Null, string> { Value = message },
                            cancellationToken);

                        _logger.LogDebug(
                            "Published notification for UserId {UserId}, Flashcard {FlashcardName}, Count {Count} to partition {Partition} at offset {Offset}",
                            notification.UserId, notification.FlashcardName, notification.DueContentCount,
                            deliveryResult.Partition, deliveryResult.Offset);
                    }
                    catch (ProduceException<Null, string> ex)
                    {
                        _logger.LogError(ex, "Failed to publish notification for UserId {UserId}, Flashcard {FlashcardId}",
                            notification.UserId, notification.FlashcardId);
                    }
                });

                await Task.WhenAll(publishTasks);
            }

            var publishDuration = DateTime.UtcNow - publishStartTime;
            _logger.LogInformation("Published {Count} notifications in {Duration}ms", notifications.Count, publishDuration.TotalMilliseconds);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("FlashcardNotificationWorker is stopping");

            try
            {
                _kafkaProducer.Flush(TimeSpan.FromSeconds(10));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error flushing Kafka producer during shutdown");
            }

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing FlashcardNotificationWorker resources");
            _kafkaProducer.Dispose();
            base.Dispose();
        }
    }
}