using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Common.Ultils;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions _snakeCaseOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static IResult Json(object data)
    {
        return Results.Json(data, _snakeCaseOptions, statusCode:200);
    }
}