using Application.Common.Models.CalendarModel;
using Application.Services.CalendarService.Interface;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Application.Services.CalendarService;

public class EventService(ICalendarService calendarService) : IEventService
{
    private readonly ICalendarService _calendarService = calendarService;
    private readonly ILogger<EventService> _logger;
    private readonly IRestClient _restClient = new RestClient("https://www.googleapis.com/calendar/v3/calendars");

    public async Task<EventResponse> CreateNextReviewDateAsync(DateTime nextReviewDate, List<string> email, string eventTitle)
    {
        EventCalendarModel eventRequest = new EventCalendarModel();
        eventRequest.start = new EventTime
        {
            dateTime = nextReviewDate,
            timeZone = "Asia/Ho_Chi_Minh"
        };
        eventRequest.end = new EventTime
        {
            dateTime = nextReviewDate.AddHours(1),
            timeZone = "Asia/Ho_Chi_Minh"
        };
        eventRequest.summary = eventTitle;
        eventRequest.description = "This is reminder for " + eventTitle;
        eventRequest.attendees = email.Select(x => new Attendee
        {
            email = x,
            displayName = x.Split('@')[0],
            responseStatus = "accepted"
        }).ToList();

        var restRequest = new RestRequest("primary/events");

        var token = await _calendarService.GetAccessTokenAsync();
        restRequest.AddQueryParameter("conferenceDataVersion", "1");
        restRequest.AddJsonBody(eventRequest);
        restRequest.AddHeader("Authorization", $"Bearer {token}");
        var response = await _restClient.PostAsync<EventResponse>(restRequest);
        return response;
    }

    //public async Task<EventResponse> CreateEventAsync(EventCalendarModel eventRequest)
    //{
    //    var restRequest = new RestRequest("primary/events");

    //    var token = await _calendarService.GetAccessTokenAsync();
    //    restRequest.AddQueryParameter("conferenceDataVersion", "1");
    //    restRequest.AddJsonBody(eventRequest);
    //    restRequest.AddHeader("Authorization", $"Bearer {token}");
    //    var response = await _restClient.PostAsync<EventResponse>(restRequest);
    //    return response;
    //}

    public async Task<EventResponse> DeleteEventAsync(string eventId)
    {
        var restRequest = new RestRequest($"primary/events/{eventId}");
        var token = await _calendarService.GetAccessTokenAsync();
        restRequest.AddHeader("Authorization", $"Bearer {token}");
        var response = await _restClient.DeleteAsync<EventResponse>(restRequest);
        return response;
    }

    public async Task<EventResponse> GetEventIdAsync(string eventId)
    {
        var restRequest = new RestRequest($"primary/events/{eventId}");

        var token = await _calendarService.GetAccessTokenAsync();

        restRequest.AddHeader("Authorization", $"Bearer {token}");

        var response = await _restClient.GetAsync<EventResponse>(restRequest);

        return response;
    }

    public async Task<PagedEventResponse> GetEventsAsync()
    {
        var restRequest = new RestRequest($"primary/events");

        var token = await _calendarService.GetAccessTokenAsync();

        restRequest.AddHeader("Authorization", $"Bearer {token}");

        var response = await _restClient.GetAsync<PagedEventResponse>(restRequest);

        return response;
    }

    public async Task<PagedEventResponse> GetEventsDateAsync(DateTime start, DateTime end)
    {
        var restRequest = new RestRequest($"primary/events");

        var token = await _calendarService.GetAccessTokenAsync();

        restRequest.AddHeader("Authorization", $"Bearer {token}");
        restRequest.AddQueryParameter("timeMin", start.ToString("yyyy-MM-dd'T'HH:mm:ssZ"));
        restRequest.AddQueryParameter("timeMax", end.ToString("yyyy-MM-dd'T'HH:mm:ssZ"));
        restRequest.AddQueryParameter("timeZone", "Asia/Ho_Chi_Minh");

        var response = await _restClient.GetAsync<PagedEventResponse>(restRequest);

        return response;
    }

    public async Task<EventResponse> UpdateEventAsync(EventCalendarModel eventRequest, string eventId)
    {
        var restRequest = new RestRequest($"primary/events/{eventId}");

        var token = await _calendarService.GetAccessTokenAsync();

        restRequest.AddHeader("Authorization", $"Bearer {token}");
        restRequest.AddJsonBody(eventRequest);

        var response = await _restClient.PutAsync<EventResponse>(restRequest);
        return response;
    }
}
