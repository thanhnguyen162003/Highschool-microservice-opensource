using Application.Common.Models.Common;
using Application.Common.Models.MailModels;
using Application.Common.Ultils;
using Application.Constants;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Common.UUID;
using Domain.Constants;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Services.Authentication;
using Domain.Services.ServiceTask.CacheCommon;
using System.Net;

namespace Application.Features.Authen.v1.Register
{
    public class RegisterCommandHandler(IAuthenticationService authenticationService,
        ICacheDataTask cacheDataTask, IProducerService producerService) : IRequestHandler<RegisterCommand, ResponseModel>
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly ICacheDataTask _cacheDataTask = cacheDataTask;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Create password hash and salt
            _authenticationService.CreatePasswordHash(request.Password!, out byte[] passwordHash, out byte[] passwordSalt);

            // Create user
            BaseUser baseUser = new BaseUser()
            {
                Id = new UuidV7().Value,
                Email = request.Email,
                RoleId = (int)EnumExtensions.ConvertToRoleValue(request.RoleName)!,
                Fullname = request.FullName,
                Provider = ProviderConstant.CREDENTIAL,
                CreatedAt = DateTime.Now,
                Status = AccountStatus.Active.ToString(),
                UpdatedAt = DateTime.Now,
                Timezone = TimeZoneInfo.Utc.ToString(),
                Password = passwordHash,
                PasswordSalt = passwordSalt,
                ProgressStage = ProgressStage.NewUser.ToString(),
                ProfilePicture = AvatarExtension.GetAvatar(request.FullName!)
            };

            // Save otp and information user to redis cache
            string otp = _authenticationService.GenerateOTP();

            _cacheDataTask.SaveAccountToVerify(baseUser, otp);

            var mailModel = new MailModel() 
            { 
                MailType = MailSendType.ConfirmAccount,
                MailConfirmModel = new MailConfirmModel()
                {
                    Email = baseUser.Email!,
                    FullName = baseUser.Fullname!,
                    OTP = otp
                }
            };
            //produce instead of queue background
            var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.MailCreated, baseUser.Id.ToString(), mailModel);
            if (result)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.Created,
                    Message = MessageCommon.CheckMailToVerifyAccount,
                    Data = baseUser.Id
                };
            }

            return new ResponseModel()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.SendMailFailed
            };
        }
    }
}
