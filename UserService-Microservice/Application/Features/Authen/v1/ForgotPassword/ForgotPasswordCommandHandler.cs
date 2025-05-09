using Application.Common.Models.Common;
using Application.Common.Models.MailModels;
using Application.Constants;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Common.Messages;
using Domain.Enumerations;
using Domain.Services.Authentication;
using Domain.Services.ServiceTask.CacheCommon;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.Authen.v1.ForgotPassword
{
    public class ForgotPasswordCommandHandler(IUnitOfWork unitOfWork,
        IAuthenticationService authenticationService, ICacheDataTask cacheDataTask, IProducerService producerService) : IRequestHandler<ForgotPasswordCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly ICacheDataTask _cacheDataTask = cacheDataTask;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(request.Email);

            if (user == null)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageUser.UserNotFound
                };
            }

            var code = _authenticationService.GenerateOTP();

            // Save OTP to cache
            _cacheDataTask.SaveResetPassword(request.Email, code);

            var mailModel = new MailModel()
            {
                MailType = MailSendType.ForgotPassword,
                MailConfirmModel = new MailConfirmModel()
                {
                    Email = request.Email,
                    OTP = code,
                    FullName = user.Fullname!
                }
            };

            var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.MailCreated, user.Id.ToString(), mailModel);
            if (result)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.SendMailSuccess,
                    Data = code
                };
            }

            return new ResponseModel()
            {
                Status = HttpStatusCode.BadRequest,
                Message = MessageCommon.SendMailFailed
            };
        }
    }
}
