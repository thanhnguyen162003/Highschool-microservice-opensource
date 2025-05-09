using System.Threading.Channels;

namespace Application.Services.KafkaService.Consumer
{
    /// <summary>
    /// This class handles the batching of incoming messages and the concurrent processing of batches by worker tasks.
    /// It uses a <see cref="Channel"/> to buffer and dispatch batches of messages to worker tasks for processing.
    /// The class allows setting a batch size and a timeout to trigger batch processing even if the batch is not full.
    /// </summary>
    /// <remarks>
    /// Initializes the batch processor with the specified batch size and timeout.
    /// </remarks>
    /// <param name="logger">The logger for logging events.</param>
    /// <param name="batchSize">The maximum number of messages per batch (default is 50).</param>
    /// <param name="batchTimeout">The maximum time to wait before processing the batch (default is 5 seconds).</param>
    public class BatchProcessor(ILogger logger, int batchSize = 50, TimeSpan? batchTimeout = null)
    {
        private readonly Channel<string[]> _batchChannel = Channel.CreateUnbounded<string[]>();   // Channel to hold batches of messages
        private readonly int _batchSize = batchSize;    // The maximum size of each batch
        private readonly TimeSpan _batchTimeout = batchTimeout ?? TimeSpan.FromSeconds(5);    // The maximum wait time before triggering batch processing
        private readonly ILogger _logger = logger;   // Logger for logging events
        private readonly List<string> _currentBatch = new List<string>();    // Holds the current batch of messages
        private DateTime _lastBatchTime = DateTime.UtcNow;    // The last time a batch was processed

        /// <summary>
        /// Adds an incoming message to the current batch. If the batch size is reached or the batch timeout
        /// is exceeded, the batch is sent to the channel for processing.
        /// </summary>
        /// <param name="message">The incoming message to add to the batch.</param>
        /// <param name="stoppingToken">The cancellation token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ProcessIncomingMessage(string message, CancellationToken stoppingToken)
        {
            _currentBatch.Add(message);

            // If batch size or timeout is reached, send the batch to the channel
            if (_currentBatch.Count >= _batchSize || DateTime.UtcNow - _lastBatchTime >= _batchTimeout)
            {
                var batch = _currentBatch.ToArray();
                _currentBatch.Clear();  // Clear the current batch after sending
                _lastBatchTime = DateTime.UtcNow;

                await _batchChannel.Writer.WriteAsync(batch, stoppingToken);
                _logger.LogInformation($"Batch with {_batchSize} messages sent to channel.");
            }
        }

        /// <summary>
        /// Worker method that processes batches of messages. This method will be executed by multiple worker tasks.
        /// It listens for new batches on the channel and processes them asynchronously.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token to stop processing when needed.</param>
        /// <param name="processBatch">The method to process each batch of messages.</param>
        /// <returns>A task representing the processing of batches.</returns>
        public async Task ProcessBatchWorker(CancellationToken stoppingToken, Func<IEnumerable<string>, Task> processBatch)
        {
            // Read batches from the channel and process them asynchronously
            await foreach (var batch in _batchChannel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await processBatch(batch);  // Process the batch using the provided method
                    _logger.LogInformation($"Processed batch with {batch.Length} messages.");
                } catch (Exception ex)
                {
                    _logger.LogError($"Error processing batch: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Marks the batch channel as completed. This will stop further writing to the channel.
        /// </summary>
        public void CompleteBatchChannel()
        {
            _batchChannel.Writer.Complete();
        }
    }

}
