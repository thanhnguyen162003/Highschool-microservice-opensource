using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Constants;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.Authen.v1.ResetPassword
{
    public class ResetPasswordCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, ICacheRepository cacheRepository) : IRequestHandler<ResetPasswordCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly ICacheRepository _cacheRepository = cacheRepository;

        public async Task<ResponseModel> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var otp = await _cacheRepository.GetAsync<string>(StorageRedis.ResetPassword, request.Email);

            if (otp == null || !otp.Equals(request.Otp))
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageUser.OTPNotValid
                };
            }

            var user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(request.Email);

            if (user == null)
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageUser.UserNotFound
                };
            }

            _authenticationService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Password = passwordHash;
            user.PasswordSalt = passwordSalt;

            _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageUser.ResetPasswordSuccess
                };
            }

            return new ResponseModel
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageUser.ResetPasswordFailed
            };

        }
    }
}
