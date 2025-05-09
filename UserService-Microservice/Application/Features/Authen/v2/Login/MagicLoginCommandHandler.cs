using Application.Common.Models.Common;
using Application.Common.Models.MailModels;
using Application.Services.MailService;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Common.Messages;
using Domain.Constants;
using Domain.Enumerations;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using System.Net;
using System.Web;

namespace Application.Features.Authen.v2.Login
{
    public class MagicLoginCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService,
        IProducerService producerService, ICacheRepository cacheRepository, IEmailService emailService) : IRequestHandler<MagicLoginCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IProducerService _producerService = producerService;
        private readonly ICacheRepository _cacheRepository = cacheRepository;
        private readonly IEmailService _emailService = emailService;

        public async Task<ResponseModel> Handle(MagicLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(request.Email);

            if (user != null)
            {
                if (user.DeletedAt != null)
                {
                    return new ResponseModel()
                    {
                        Status = HttpStatusCode.Forbidden,
                        Message = MessageUser.UserDeleted
                    };
                } else if (user.Status!.Equals(AccountStatus.Blocked.ToString()))
                {
                    return new ResponseModel()
                    {
                        Status = HttpStatusCode.Forbidden,
                        Message = MessageUser.UserBlocked
                    };
                }
            }

            // Save token to redis cache
            var token = _authenticationService.GenerateRefreshToken();
            string encodedToken = HttpUtility.UrlEncode(token)!;
            var fullKey = $"{request.Email}:login";

            await _cacheRepository.SetAsync(StorageRedis.VerifyAccount, fullKey, token, 10);

            // Send mail
            var mailConfirm = new MailConfirmModel()
            {
                Email = request.Email!,
                FullName = user == null ? request.Email : user.Fullname!,
                Token = $"{UrlConstant.UrlVerifyLogin}?token={encodedToken}&email={request.Email}"
            };

            var result = await _emailService.SendEmailConfirm(mailConfirm);
            if (result)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageUser.SendMailSuccess
                };
            }

            return new ResponseModel()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageUser.SendMailFailed
            };

        }
    }
}
