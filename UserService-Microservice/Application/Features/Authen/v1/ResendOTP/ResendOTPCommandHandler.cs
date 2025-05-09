using Application.Common.Models.Common;
using Application.Common.Models.MailModels;
using Application.Constants;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Common.Messages;
using Domain.Enumerations;
using Domain.Services.Authentication;
using System.Net;

namespace Application.Features.Authen.v1.ResendOTP
{
    public class ResendOTPCommandHandler(IAuthenticationService authenticationService, IProducerService producerService) : IRequestHandler<ResendOTPCommand, ResponseModel>
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(ResendOTPCommand request, CancellationToken cancellationToken)
        {
            var mail = new MailModel()
            {
                MailType = MailSendType.ResendMail,
                MailConfirmModel = new MailConfirmModel
                {
                    Email = request.Email,
                    OTP = _authenticationService.GenerateOTP()
                }
            };

            var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.MailCreated, Guid.NewGuid().ToString(), mail);
            if (result)
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.SendMailSuccess
                };
            }

            return new ResponseModel
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.SendMailFailed
            };
        }
    }
}
