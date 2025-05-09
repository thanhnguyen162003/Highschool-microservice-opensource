using Domain.Enums;

namespace Application.Common.Models.ProduceModels
{
    public class MailModel
    {
        public MailSendType MailType { get; set; }
        //public MailInviteMemberModel? MailInviteMemberModel { get; set; }
        public IEnumerable<MailInviteMemberModel> MailInviteMemberModels { get; set; } = new List<MailInviteMemberModel>();
    }
}
