namespace Application.Common.Models.MailModels
{
    public class MailConfirmModel
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? OTP { get; set; }
        public string? Token { get; set; }
    }
}
