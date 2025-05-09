namespace Application.Common.Models.MailModels
{
    public class MailAssignmentNotificationModel
    {
        public Guid ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public string TotalTime { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string TotalQuestion { get; set; } = string.Empty;
        public string Noticed { get; set; } = string.Empty;
    }
}
