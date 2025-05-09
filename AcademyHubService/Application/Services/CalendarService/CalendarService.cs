
using System.Text.Json;
using Application.Common.Models.CalendarModel;
using Application.Services.CalendarService.Interface;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Application.Services.CalendarService;

public class CalendarService : ICalendarService
{
    private readonly CalendarSetting _setting;
    private readonly ILogger<CalendarService> _logger;
    private readonly IRestClient _restClient;
    private readonly string tokenFilePath = ".\\token.json";
    public CalendarService(IOptions<CalendarSetting> options, ILogger<CalendarService> logger)
    {
        _setting = options.Value;
        _restClient = new RestClient($"{_setting.TokenUri}");
    }

    public string Authorize()
    {
        var url = $"{_setting.AuthUrl}?" +
            $"scope={_setting.Scope}" +
                $"&access_type=offline" +
                $"&response_type=code" +
                $"&state=lehoangson2259" +
                $"&redirect_uri={_setting.RedirectUri}" +
                $"&client_id={_setting.ClientId}";
        return url;
    }

    public Task<string> CreateEventAsync(CalendarTokenModel token, string eventData)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteEventAsync(CalendarTokenModel token, string eventId)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var token = this.GetToken();
        if (token.IsExpired)
        {
            token = await this.RefreshTokenAsync(token.refresh_token);
        }
        return token.access_token;
    }

    public Task<string> GetEventsAsync(CalendarTokenModel token, DateTime start, DateTime end)
    {
        throw new NotImplementedException();
    }

    public async Task<CalendarTokenModel> GetTokenAsync(string code)
    {
        var request = new RestRequest();
        request.AddParameter("code", code);
        request.AddParameter("client_id", _setting.ClientId);
        request.AddParameter("client_secret", _setting.ClientSecret);
        request.AddParameter("redirect_uri", _setting.RedirectUri);
        request.AddParameter("grant_type", "authorization_code");
        var token =  await _restClient.PostAsync<CalendarTokenModel>(request);

        await this.SaveTokenAsync(token);
        return token;
    }

    public async Task SaveTokenAsync(CalendarTokenModel token)
    {
        await System.IO.File.WriteAllTextAsync(this.tokenFilePath, JsonSerializer.Serialize(token));
    }

    public CalendarTokenModel GetToken()
    {
        var token = System.IO.File.ReadAllText(this.tokenFilePath);
        var response = JsonSerializer.Deserialize<CalendarTokenModel>(token);
        return response;
    }

    public async Task<CalendarTokenModel> RefreshTokenAsync(string refreshToken)
    {
        var token = this.GetToken();
        var request = new RestRequest();
        request.AddParameter("refresh_token", token.refresh_token);
        request.AddParameter("client_id", _setting.ClientId);
        request.AddParameter("client_secret", _setting.ClientSecret);
        request.AddParameter("redirect_uri", _setting.RedirectUri);
        request.AddParameter("grant_type", "refresh_token");

        var response = await _restClient.PostAsync<CalendarTokenModel>(request);
        return response;
    }

    public Task<string> UpdateEventAsync(CalendarTokenModel token, string eventId, string eventData)
    {
        throw new NotImplementedException();
    }
}
