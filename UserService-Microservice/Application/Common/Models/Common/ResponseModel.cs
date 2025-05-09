using System.Net;
using System.Text.Json;

namespace Application.Common.Models.Common;

public class ResponseModel
{
	public HttpStatusCode? Status { get; set; }
	public string? Message { get; set; }
	public object? Data { get; set; }

	public ResponseModel()
	{
	}
	public ResponseModel(HttpStatusCode? status, string? message, object? data)
	{
		Status = status;
		Message = message;
		Data = data;
	}

	public ResponseModel(HttpStatusCode? status, string? message)
	{
		Status = status;
		Message = message;
	}

	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}
}