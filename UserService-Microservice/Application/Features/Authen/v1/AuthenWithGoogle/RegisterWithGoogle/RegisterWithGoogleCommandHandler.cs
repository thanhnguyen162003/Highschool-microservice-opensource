using Application.Common.Models.AuthenModel;
using Application.Common.Models.Common;
using Application.Common.Ultils;
using Domain.Common.Messages;
using Domain.Common.UUID;
using Domain.Constants;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.Authen.v1.AuthenWithGoogle.RegisterWithGoogle
{
    public class RegisterWithGoogleCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, IMapper mapper) : IRequestHandler<RegisterWithGoogleCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(RegisterWithGoogleCommand request, CancellationToken cancellationToken)
        {
            // Create password hash and salt
            _authenticationService.CreatePasswordHash(out byte[] passwordHash, out byte[] passwordSalt);

            // Create user
            BaseUser baseUser = new BaseUser()
            {
                Id = new UuidV7().Value,
                Email = request.Email,
                RoleId = (int)RoleEnum.Unknown,
                Fullname = request.FullName,
                Provider = ProviderConstant.GOOGLE,
                CreatedAt = DateTime.Now,
                Status = AccountStatus.Active.ToString(),
                UpdatedAt = DateTime.Now,
                Timezone = TimeZoneInfo.Utc.ToString(),
                Password = passwordHash,
                PasswordSalt = passwordSalt,
                ProgressStage = ProgressStage.NewUser.ToString(),
                ProfilePicture = request.Avatar ?? AvatarExtension.GetAvatar(request.FullName!),
                LastLoginAt = DateTime.Now,
                Username = request.Email!.ToSplitUsername()
            };

            // Add user to database
            await _unitOfWork.UserRepository.AddAsync(baseUser);

            // Create session
            var deviceInfo = _authenticationService.GetDeviceInformation();
            var session = new Session
            {
                Id = new UuidV7().Value,
                UserId = baseUser.Id,
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

            var userLogin = _mapper.Map<LoginResponseModel>(baseUser);
            userLogin.RefreshToken = session.RefreshToken;
            userLogin.SessionId = session.Id;
            var (accessToken, timeExpire) = _authenticationService.GenerateAccessToken(baseUser, session.Id);
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
