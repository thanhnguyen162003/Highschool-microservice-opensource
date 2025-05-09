using Domain.Enumerations;

namespace Application.Common.Models.MailModels
{
    public class MailModel
    {
        public MailSendType MailType { get; set; }
        public MailConfirmModel? MailConfirmModel { get; set; } 
        public IEnumerable<MailInviteMemberModel>? MailInviteMemberModels { get; set; }
        public MailAssignmentNotificationModel? MailAssignmentNotificationModel { get; set; }
    }
}
