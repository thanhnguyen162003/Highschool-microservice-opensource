using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Mail;

namespace Application.Common.Models.CalendarModel;

public class EventCalendarCreateModel
{
    public EventTimeRequest start { get; set; }

    public EventTimeRequest end { get; set; }
    public string? summary { get; set; }


    public string? description { get; set; }


    public List<AttendeeCreateRequest>? attendees { get; set; }
    public ConferenceDataCreateRequest? conferenceData { get; set; }

    public Reminders? reminders { get; set; }


}

public class ConferenceDataCreateRequest
{
    public CreateRequestForConferenceData createRequest { get; set; }
}

public class CreateRequestForConferenceData
{
    public string requestId { get; set; }
    public ConferenceSolutionKey conferenceSolutionKey { get; set; }

}
public class EventTimeRequest
{
    public DateTime? dateTime { get; set; }
    public string timeZone { get; set; }
}
public class AttendeeCreateRequest
{
    public string email { get; set; }
    public string displayName { get; set; }
    public string responseStatus { get; set; }
}

