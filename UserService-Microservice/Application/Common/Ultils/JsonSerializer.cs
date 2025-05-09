using Confluent.Kafka;
using System.Text.Json;

namespace Domain.Common.Ultils;

public class JsonSerializer<T> : ISerializer<T>
{
	public byte[] Serialize(T data, SerializationContext context)
	{
		return JsonSerializer.SerializeToUtf8Bytes(data);
	}
}