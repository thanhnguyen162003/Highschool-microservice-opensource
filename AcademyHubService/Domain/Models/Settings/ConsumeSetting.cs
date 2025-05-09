namespace Domain.Models.Settings
{
    public class ConsumeSetting
    {
        /// <summary>
        /// The Kafka topic to subscribe to.
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// The number of worker tasks to process batches (default is 3).
        /// </summary>
        public int WorkerCount { get; set; } = 3;

        /// <summary>
        /// Timeout for consuming messages from Kafka.
        /// </summary>
        public int TimeoutMessageKafka { get; set; } = 1000;

        /// <summary>
        /// Timeout for consuming messages from Kafka again.
        /// </summary>
        public int TimeConsumeAgain { get; set; } = 3000;
    }
}
