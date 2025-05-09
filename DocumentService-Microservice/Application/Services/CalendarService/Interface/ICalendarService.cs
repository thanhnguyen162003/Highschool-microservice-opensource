using Application.Common.Models.CalendarModel;

namespace Application.Services.CalendarService.Interface;

public interface ICalendarService
{
    Task<CalendarTokenModel> GetTokenAsync(string code);
    string Authorize();
    Task SaveTokenAsync(CalendarTokenModel token);
    CalendarTokenModel GetToken();
    Task<string> GetAccessTokenAsync();
    Task<CalendarTokenModel> RefreshTokenAsync(string refreshToken);
    Task<string> GetEventsAsync(CalendarTokenModel token, DateTime start, DateTime end);
    Task<string> CreateEventAsync(CalendarTokenModel token, string eventData);
    Task<string> UpdateEventAsync(CalendarTokenModel token, string eventId, string eventData);
    Task<string> DeleteEventAsync(CalendarTokenModel token, string eventId);
}
