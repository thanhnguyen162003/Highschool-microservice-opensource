using System.Text.Json;

namespace Domain.Models.Common
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Location { get; set; }
        public string? Detail { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public string GetError()
        {
            return
                "StatusCode: " + StatusCode + "\n" +
                "Error: " + Message + "\n" +
                "Location: " + Location + "\n" +
                "Detail: " + Detail + "\n";
        }
    }
}
