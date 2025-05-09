using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Mail;

namespace Application.Common.Models.CalendarModel;

public class EventCalendarModel
{
    public EventCalendarModel()
    {
        this.eventType = "default";
        this.visibility = "default";
        this.status = "confirmed";
        this.transparency = "opaque";
    }

    public EventTime start { get; set; }

    public EventTime end { get; set; }
    public string? summary { get; set; }


    public string? description { get; set; }


    public bool? anyoneCanAddSelf { get; set; }


    public List<Attachment>? attachments { get; set; }


    public List<Attendee>? attendees { get; set; }

    public BirthdayProperties? birthdayProperties { get; set; }

    public string? colorId { get; set; }

    public ConferenceData? conferenceData { get; set; }


    public string? eventType { get; set; }

    public ExtendedProperties? extendedProperties { get; set; }

    public FocusTimeProperties? focusTimeProperties { get; set; }

    public Gadget? gadget { get; set; }


    public bool? guestsCanInviteOthers { get; set; }

    public bool? guestsCanModify { get; set; }


    public bool? guestsCanSeeOtherGuests { get; set; }


    public string? id { get; set; }

    public string? location { get; set; }

    public EventTime? originalStartTime { get; set; }
    public OutOfOfficeProperties? outOfOfficeProperties { get; set; }


    public List<string>? recurrence { get; set; }

    public Reminders? reminders { get; set; }


    public int? sequence { get; set; }


    public Source? source { get; set; }

    public string? status { get; set; }

    public string? transparency { get; set; }


    public string? visibility { get; set; }

    public WorkingLocationProperties? workingLocationProperties { get; set; }

}
public class EventResponse
{
    public string? id { get; set; }

    public string? status { get; set; }

    public string? htmlLink { get; set; }

    public DateTime? created { get; set; }

    public DateTime? updated { get; set; }

    public string? summary { get; set; }

    public string? description { get; set; }

    public string? location { get; set; }

    public string? colorId { get; set; }

    public Creator? creator { get; set; }

    public Organizer? organizer { get; set; }

    public EventTime start { get; set; }

    public EventTime end { get; set; }

    public bool? endTimeUnspecified { get; set; }

    public List<string>? recurrence { get; set; }

    public string? recurringEventId { get; set; }

    public EventTime? originalStartTime { get; set; }

    public string? transparency { get; set; }

    public string? visibility { get; set; }

    public string? iCalUID { get; set; }
    public int? sequence { get; set; }

    public List<Attendee>? attendees { get; set; }

    public bool? attendeesOmitted { get; set; }

    public ExtendedProperties? extendedProperties { get; set; }

    public string? hangoutLink { get; set; }

    public ConferenceData? conferenceData { get; set; }

    public Gadget? gadget { get; set; }

    public bool? anyoneCanAddSelf { get; set; }

    public bool? guestsCanInviteOthers { get; set; }

    public bool? guestsCanModify { get; set; }

    public bool? guestsCanSeeOtherGuests { get; set; }

    public bool? privateCopy { get; set; }

    public bool? locked { get; set; }

    public Reminders? reminders { get; set; }

    public Source? source { get; set; }

    public WorkingLocationProperties? workingLocationProperties { get; set; }

    public OutOfOfficeProperties? outOfOfficeProperties { get; set; }

    public FocusTimeProperties? focusTimeProperties { get; set; }

    public List<Attachment>? attachments { get; set; }

    public BirthdayProperties? birthdayProperties { get; set; }

    public string? eventType { get; set; }
}

public class PagedEventResponse
{
    public string kind { get; set; }
    public string etag { get; set; }
    public string summary { get; set; }
    public string description { get; set; }
    public DateTime updated { get; set; }
    public string timeZone { get; set; }
    public string accessRole { get; set; }
    public List<DefaultReminder> defaultReminders { get; set; }
    public string nextPageToken { get; set; }
    public string nextSyncToken { get; set; }
    public List<EventResponse> items { get; set; }
}

public class DefaultReminder
{
    public string method { get; set; }
    public int minutes { get; set; }
}
public class Creator
{
    public string id { get; set; }
    public string email { get; set; }
    public string displayName { get; set; }
    public bool self { get; set; }
}

public class Organizer
{
    public string id { get; set; }

    public string email { get; set; }

    public string displayName { get; set; }

    public bool self { get; set; }
}

public class EventTime
{
    public DateTime? date { get; set; }
    public DateTime? dateTime { get; set; }
    public string timeZone { get; set; }
}

public class Attendee
{
    public string? id { get; set; }
    public string? email { get; set; }
    public string? displayName { get; set; }
    public bool? organizer { get; set; }
    public bool? self { get; set; }
    public bool? resource { get; set; }
    public bool? optional { get; set; }
    public string? responseStatus { get; set; }
    public string? comment { get; set; }
    public int? additionalGuests { get; set; }
}

public class ExtendedProperties
{
    public Dictionary<string, string> @private { get; set; }
    public Dictionary<string, string> shared { get; set; }
}

public class ConferenceData
{
    public CreateRequest createRequest { get; set; }
    public List<EntryPoint>? entryPoints { get; set; }
    public ConferenceSolution? conferenceSolution { get; set; }
    public string? conferenceId { get; set; }
    public string? signature { get; set; }
    public string? notes { get; set; }
}
public class CreateRequest
{
    public string requestId { get; set; }
    public ConferenceSolutionKey conferenceSolutionKey { get; set; }
    public Status? status { get; set; }
}

public class ConferenceSolutionKey
{
    public string type { get; set; }
}

public class Status
{
    public string statusCode { get; set; }
}

public class EntryPoint
{
    public string entryPointType { get; set; }

    public string uri { get; set; }

    public string label { get; set; }

    public string pin { get; set; }

    public string accessCode { get; set; }

    public string meetingCode { get; set; }

    public string passcode { get; set; }

    public string password { get; set; }
}

public class ConferenceSolution
{
    public ConferenceSolutionKey key { get; set; }
    public string name { get; set; }
    public string iconUri { get; set; }
}

public class Gadget
{
    public string type { get; set; }
    public string title { get; set; }
    public string link { get; set; }
    public string iconLink { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public string display { get; set; }
    public Dictionary<string, string> preferences { get; set; }
}

public class Reminders
{
    public bool useDefault { get; set; }
    public List<Override> overrides { get; set; }
}

public class Override
{
    public string method { get; set; }
    public int minutes { get; set; }
}

public class Source
{
    public string url { get; set; }
    public string title { get; set; }
}

public class WorkingLocationProperties
{
    public string type { get; set; }
    public object homeOffice { get; set; }
    public CustomLocation customLocation { get; set; }
    public OfficeLocation officeLocation { get; set; }
}

public class CustomLocation
{
    public string label { get; set; }
}


public class OfficeLocation
{
    public string buildingId { get; set; }
    public string floorId { get; set; }
    public string floorSectionId { get; set; }
    public string deskId { get; set; }
    public string label { get; set; }
}

public class OutOfOfficeProperties
{
    public string autoDeclineMode { get; set; }
    public string declineMessage { get; set; }
}

public class FocusTimeProperties
{
    public string autoDeclineMode { get; set; }
    public string declineMessage { get; set; }
    public string chatStatus { get; set; }
}

public class Attachment
{
    public string fileUrl { get; set; }
    public string title { get; set; }
    public string mimeType { get; set; }
    public string iconLink { get; set; }
    public string fileId { get; set; }
}

public class BirthdayProperties
{
    public string contact { get; set; }
    public string type { get; set; }
    public string customTypeName { get; set; }
}