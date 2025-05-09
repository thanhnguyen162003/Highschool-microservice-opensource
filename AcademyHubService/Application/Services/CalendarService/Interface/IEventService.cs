using Application.Common.Models.CalendarModel;

namespace Application.Services.CalendarService.Interface;

public interface IEventService
{
    Task<EventResponse> CreateNextReviewDateAsync(DateTime nextReviewDate, List<string> email, string eventTitle);
    //Task<EventResponse> CreateEventAsync(EventCalendarModel eventRequest);
    Task<EventResponse> UpdateEventAsync(EventCalendarModel eventRequest, string eventId);
    Task<EventResponse> DeleteEventAsync(string eventId);
    Task<EventResponse> GetEventIdAsync(string eventId);
    Task<PagedEventResponse> GetEventsDateAsync(DateTime start, DateTime end);
    Task<PagedEventResponse> GetEventsAsync();
}
