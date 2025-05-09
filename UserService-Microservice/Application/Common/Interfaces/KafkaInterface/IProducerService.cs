namespace Domain.Common.Interfaces.KafkaInterface;

public interface IProducerService
{
	Task ProduceAsync(string topic, string message);
	Task ProduceWithKeyAsync(string topic, string key, string message);
	Task<bool> ProduceObjectWithKeyAsync<T>(string topic, string key, T obj);
	Task<bool> ProduceObjectAsync<T>(string topic, T obj);
	IEnumerable<string> GetTopics();
}