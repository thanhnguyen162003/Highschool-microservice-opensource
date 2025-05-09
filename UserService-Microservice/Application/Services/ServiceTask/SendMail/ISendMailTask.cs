using Application.Common.Models.MailModels;

namespace Domain.Services.ServiceTask.SendMail
{
    public interface ISendMailTask
    {
        void SendMailVerify(MailConfirmModel mail);
        void ReSendMailVerify(MailConfirmModel mail);
    }
}