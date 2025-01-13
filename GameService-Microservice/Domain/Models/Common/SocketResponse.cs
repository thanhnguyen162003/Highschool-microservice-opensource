using System.Text.Json;

namespace Domain.Models.Common
{
    public class SocketResponse<T>
    {
        public string? Type { get; set; } = string.Empty;
        public T? Data { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class SocketResponse : SocketResponse<object>
    {
        public SocketResponse() { }
    }
}
