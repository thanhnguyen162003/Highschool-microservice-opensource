namespace Application.Common.Models.MailModels
{
    public class MailInviteMemberModel
    {
        public string Email { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string LogoUrl { get; set; } = string.Empty;

        public string BannerUrl { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string ZoneName { get; set; } = string.Empty;

        public Guid CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string AcceptLink { get; set; } = string.Empty;

        public string RejectLink { get; set; } = string.Empty;
    }
}
