using Application.Common.Models.MailModels;
using Domain.Constants;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Application.Services.MailService
{
    public class EmailService(IFluentEmail fluentEmail) : IEmailService
	{

		private readonly IFluentEmail _fluentEmail = fluentEmail;

        public async Task<bool> SendEmailConfirm(MailConfirmModel mailDTO)
		{
			var renderedTemplate = (mailDTO.Token != null) ?
                MailConstant.ConfirmMail.TemplatePathToken
                                    .Replace("@Model.UserName", mailDTO.FullName)
                                    .Replace("@Model.Token", mailDTO.Token)

				: 
				
				MailConstant.ConfirmMail.TemplatePathOTP
									.Replace("@Model.UserName", mailDTO.FullName)
									.Replace("@Model.OTP", mailDTO.OTP);

			var response = await _fluentEmail.To(mailDTO.Email)
										 .Subject(MailConstant.ConfirmMail.Title)
										 .Body(renderedTemplate, isHtml: true)
										 .SendAsync();

			return response.Successful;
		}

		public async Task<bool> SendEmailInviteMember(MailInviteMemberModel mailDTO, string authorName, string studentName)
        {
            var renderedTemplate = MailConstant.InviteMember.TemplatePath
                                    .Replace("@Model.Description", mailDTO.Description)
                                    .Replace("@Model.Role", mailDTO.Type)
                                    .Replace("@Model.LogoUrl", mailDTO.LogoUrl)
                                    .Replace("@Model.BannerUrl", mailDTO.BannerUrl)
                                    .Replace("@Model.StudentName", studentName)
                                    .Replace("@Model.ZoneName", mailDTO.ZoneName)
                                    .Replace("@Model.CreatedBy", authorName)
                                    .Replace("@Model.CreatedAt", mailDTO.CreatedAt.ToString("dd/MM/yyyy"))
                                    .Replace("@Model.AcceptLink", mailDTO.AcceptLink)
                                    .Replace("@Model.RejectLink", mailDTO.RejectLink);
            var response = await _fluentEmail.To(mailDTO.Email)
                                         .Subject(MailConstant.InviteMember.Title)
                                         .Body(renderedTemplate, isHtml: true)
                                         .SendAsync();
            return response.Successful;
        }

        public async Task<bool> SendEmailAssignmentNotification(MailAssignmentNotificationModel mailDTO, IEnumerable<Address> students)
        {
            var renderedTemplate = MailConstant.NotificationAssignment.TemplatePath
                                    .Replace("@Model.Type", mailDTO.Type)
                                    .Replace("@Model.ZoneName", mailDTO.ZoneName)
                                    .Replace("@Model.Title", mailDTO.Title)
                                    .Replace("@Model.DueDate", mailDTO.DueDate)
                                    .Replace("@Model.TotalTime", mailDTO.TotalTime)
                                    .Replace("@Model.TotalQuestion", mailDTO.TotalQuestion)
                                    .Replace("@Model.Noticed", mailDTO.Noticed);

            var response = await _fluentEmail.To(students)
                                         .Subject(MailConstant.NotificationAssignment.Title)
                                         .Body(renderedTemplate, isHtml: true)
                                         .SendAsync();

            return response.Successful;
        }

    }
}
