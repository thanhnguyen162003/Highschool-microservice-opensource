using Application.Constants;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System.Diagnostics;

namespace Application.TransactionalOutbox
{
    public class OutboxMessageProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxMessageProcessor> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly int _batchSize = 100;
        private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(5);
        
        public OutboxMessageProcessor(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxMessageProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            
            // Create a retry policy for transient failures
            _retryPolicy = Policy
                .Handle<Exception>(ex => IsTransientException(ex))
                .WaitAndRetryAsync(
                    3,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (exception, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            exception, 
                            "Error publishing outbox message. Retrying ({RetryCount}/3) after {RetryTimespan}s", 
                            retryCount, 
                            timespan.TotalSeconds);
                    });
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Outbox message processor started");
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await PublishOutboxMessagesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing outbox messages");
                }

                await Task.Delay(_processingInterval, cancellationToken);
            }
            
            _logger.LogInformation("Outbox message processor stopped");
        }

        private async Task PublishOutboxMessagesAsync(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            int processedCount = 0;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                await using var dbContext = scope.ServiceProvider.GetRequiredService<UserDatabaseContext>();
                var producer = scope.ServiceProvider.GetRequiredService<IProducerService>();

                // Process messages in batches until no more pending messages
                bool hasMoreMessages;
                do
                {
                    // Get batch of pending messages - note we're looking for NOT dispatched messages
                    var messages = await dbContext.OutboxMessages
                        .Where(om => !om.IsMessageDispatched)
                        .OrderBy(om => om.OccurredOn)
                        .Take(_batchSize)
                        .ToListAsync(cancellationToken);

                    hasMoreMessages = messages.Count == _batchSize;
                    
                    if (messages.Any())
                    {
                        _logger.LogInformation("Processing batch of {Count} outbox messages", messages.Count);
                        await ProcessMessageBatchAsync(messages, dbContext, producer, cancellationToken);
                        processedCount += messages.Count;
                    }
                } 
                while (hasMoreMessages && !cancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process outbox messages");
                throw;
            }

            stopwatch.Stop();
            if (processedCount > 0)
            {
                _logger.LogInformation("Processed {Count} outbox messages in {ElapsedMs}ms", 
                    processedCount, stopwatch.ElapsedMilliseconds);
            }
        }

        private async Task ProcessMessageBatchAsync(
            List<OutboxMessage> messages, 
            UserDatabaseContext dbContext,
            IProducerService producer, 
            CancellationToken cancellationToken)
        {
            foreach (var outboxMessage in messages)
            {
                try
                {
                    // Use retry policy when publishing message
                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        await producer.ProduceObjectWithKeyAsync(
                            TopicKafkaConstaints.MajorSelected, 
                            outboxMessage.EventId.ToString(), 
                            outboxMessage);
                    });

                    // Mark as processed
                    outboxMessage.IsMessageDispatched = true;
                    outboxMessage.ProcessedOn = DateTime.UtcNow;
                    
                    // Save changes for each message to avoid losing progress if a later message fails
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "Failed to process outbox message {MessageId}. Will retry on next batch.", 
                        outboxMessage.EventId);
                }
            }
        }

        private bool IsTransientException(Exception ex)
        {
            // Identify transient exceptions that should be retried
            // This needs to be customized based on the specific exceptions your system encounters
            return ex is TimeoutException 
                || ex is System.Net.Sockets.SocketException
                || ex.Message.Contains("transient");
        }
    }
}
