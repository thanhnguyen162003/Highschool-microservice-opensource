namespace Application.Common.Interfaces.KafkaInterface;

public interface IProducerService
{
    Task<bool> ProduceAsync(string topic, string message);
    Task<bool> ProduceWithKeyAsync(string topic, string key, string message);
    Task<bool> ProduceObjectWithKeyAsync<T>(string topic,string key, T obj);
    Task<bool> ProduceObjectAsync<T>(string topic, T obj);
}