using Application.Common.Models.MailModels;
using FluentEmail.Core.Models;

namespace Application.Services.MailService
{
    public interface IEmailService
	{
		Task<bool> SendEmailConfirm(MailConfirmModel mailDTO);
        Task<bool> SendEmailInviteMember(MailInviteMemberModel mailDTO, string authorName, string studentName);
        Task<bool> SendEmailAssignmentNotification(MailAssignmentNotificationModel mailDTO, IEnumerable<Address> students);

    }
}