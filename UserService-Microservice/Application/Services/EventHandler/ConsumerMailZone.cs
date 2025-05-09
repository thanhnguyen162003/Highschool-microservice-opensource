using Application.Common.Kafka;
using Application.Common.Models.Common;
using Application.Common.Models.MailModels;
using Application.Constants;
using Application.Services.MailService;
using Domain.Common.Messages;
using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Repositories.Interfaces;
using System.Text.Json;

namespace Application.Services.EventHandler
{
    public class ConsumerMailZone(IConfiguration configuration, ILogger<ConsumerMailZone> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<MailModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.MailZoneCreated, "user_service_group")
    {
        protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            var emailService = serviceProvider.GetRequiredService<IEmailService>();;

            MailModel mailModel = JsonSerializer.Deserialize<MailModel>(message)!;
            
            switch (mailModel.MailType)
            {
                case MailSendType.InviteMember:
                {
                    foreach(var member in mailModel.MailInviteMemberModels ?? Enumerable.Empty<MailInviteMemberModel>())
                    {
                        var author = await unitOfWork.UserRepository.GetDetailUser(member.CreatedBy);
                        var student = await unitOfWork.UserRepository.GetUserByUsernameOrEmail(member.Email);

                        if (author == null)
                        {
                            Console.WriteLine(MessageCommon.NotFound);
                            return;
                        }

                        var emailSent = await emailService.SendEmailInviteMember(member, author?.Fullname!, student == null ? member.Email : student.Fullname!);
                        if (emailSent)
                        {
                            Console.WriteLine(MessageCommon.SendMailSuccess);
                            return;
                        }
                    }

                    break;
                }
                case MailSendType.NotificationSubmit:
                {
                        //var student  = await unitOfWork.UserRepository.GetUserByUsernameOrEmail(email);
                        //if (student == null)
                        //{
                        //    Console.WriteLine(MessageCommon.NotFound);
                        //    return;
                        //}
                        //var emailSent = await emailService.SendEmailAssignmentNotification(mailModel.MailAssignmentNotificationModel!, student.Fullname!, email);
                        //if (emailSent)
                        //{
                        //    Console.WriteLine(MessageCommon.SendMailSuccess);
                        //    return;
                        //}
                    
                    break;
                }   
            }

            Console.WriteLine(MessageCommon.SendMailFailed);

        }
    }
}
