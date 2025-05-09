using Application.Common.Models.AuthenModel;
using Application.Common.Models.Common;
using Application.Services.ServiceTask.Common;
using Domain.Common.Messages;
using Domain.Common.UUID;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.Authen.v1.AuthenWithGoogle.LoginWithGoogle
{
    public class LoginWithGoogleCommandHandler(IAuthenticationService authenticationService, IUnitOfWork unitOfWork,
            IMapper mapper, ICommonTask commonTask) : IRequestHandler<LoginWithGoogleCommand, ResponseModel>
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ICommonTask _commonTask = commonTask;

        public async Task<ResponseModel> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
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
            else if (user.DeletedAt != null)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.Forbidden,
                    Message = MessageUser.UserDeleted
                };
            }
            else if (user.Status!.Equals(AccountStatus.Blocked.ToString()))
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.Forbidden,
                    Message = MessageUser.UserBlocked
                };
            }

            var deviceInfo = _authenticationService.GetDeviceInformation();
            var session = new Session
            {
                Id = new UuidV7().Value,
                UserId = user.Id,
                RefreshToken = _authenticationService.GenerateRefreshToken() ?? "",
                ExpiredAt = DateTime.Now.AddMinutes(30),
                CreatedAt = DateTime.Now,
                DeviceInfo = deviceInfo.PlatformName,
                IpAddress = deviceInfo.RemoteIpAddress,
                IsRevoked = false
            };

            await _unitOfWork.SessionRepository.AddAsync(session);


            if (!await _unitOfWork.SaveChangesAsync())
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = MessageCommon.ServerError
                };
            }

            _commonTask.UpdateInfoBaseUser(user.Id);

            var userLogin = _mapper.Map<LoginResponseModel>(user);
            userLogin.RefreshToken = session.RefreshToken;
            userLogin.SessionId = session.Id;
            var (accessToken, timeExpire) = _authenticationService.GenerateAccessToken(user, session.Id);
            userLogin.AccessToken = accessToken;
            userLogin.ExpiresAt = timeExpire;

            return new ResponseModel()
            {
                Status = HttpStatusCode.OK,
                Message = MessageUser.LoginSuccess,
                Data = userLogin
            };
        }

    }
}
