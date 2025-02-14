namespace Application.Common.Interfaces.KafkaInterface;

public interface IProducerBatchService
{
    /// <summary>
    /// Queues a message for batch production.
    /// </summary>
    /// <typeparam name="T">The type of the object to produce.</typeparam>
    /// <param name="topic">The Kafka topic to which the message should be produced.</param>
    /// <param name="key">The key associated with the message.</param>
    /// <param name="obj">The object to serialize and produce.</param>
    void QueueMessage<T>(string topic, string key, T obj);

    /// <summary>
    /// Flushes all buffered messages asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous flush operation.</returns>
    Task FlushAsync();
}
