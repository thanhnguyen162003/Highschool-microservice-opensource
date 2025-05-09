using Application.Common.Models.AuthenModel;
using Application.Common.Models.Common;
using Application.Common.Ultils;
using Application.Constants;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Common.Messages;
using Domain.Common.UUID;
using Domain.Constants;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Services.Authentication;
using Domain.Services.ServiceTask.CacheCommon;
using Infrastructure.Repositories.Interfaces;
using SharedProject.Models;
using System.Net;

namespace Application.Features.Authen.v2.VerifyAccount
{
    public class VerifyAccountLoginCommandHandler(ICacheRepository cacheRepository, IUnitOfWork unitOfWork,
        ICacheDataTask cacheDataTask, IAuthenticationService authenticationService, IMapper mapper, IProducerService producerService) : IRequestHandler<VerifyAccountLoginCommand, ResponseModel>
    {
        private readonly ICacheRepository _cacheRepository = cacheRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICacheDataTask _cacheDataTask = cacheDataTask;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IMapper _mapper = mapper;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(VerifyAccountLoginCommand request, CancellationToken cancellationToken)
        {
            var fullKeyOTP = $"{request.Email}:login";
            var token = await _cacheRepository.GetAsync<string>(StorageRedis.VerifyAccount, fullKeyOTP);
            if (token == null)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageUser.TokenExpired
                };
            }
            else if (!request.Token.Equals(token))
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageUser.TokenInvalid
                };
            }

            // Add token to database
            var user = await _unitOfWork.UserRepository.GetUserByUsername(request.Email);

            // Not exist user
            var username = request.Email.ToSplitUsername();
            if(user == null)
            {
                user = new BaseUser()
                {
                    Id = new UuidV7().Value,
                    Username = username,
                    Email = request.Email,
                    RoleId = (int)RoleEnum.Unknown,
                    Status = AccountStatus.Active.ToString(),
                    CreatedAt = DateTime.Now,
                    ProgressStage = ProgressStage.NewUser.ToString(),
                    LastLoginAt = DateTime.Now,
                    Provider = ProviderConstant.CREDENTIAL,
                    ProfilePicture = AvatarExtension.GetAvatar(username),
                    Fullname = username
                };

                await _unitOfWork.UserRepository.AddAsync(user);
            } else
            {
                user.LastLoginAt = DateTime.Now;

                _unitOfWork.UserRepository.Update(user);
            }


            // Exist user
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

            if (await _unitOfWork.SaveChangesAsync())
            {
                // Remove cache account to verify
                _cacheDataTask.RemoveAccountLogin(user.Email!);

                // Generate response login
                var response = _mapper.Map<LoginResponseModel>(user);
                response.RefreshToken = session.RefreshToken;
                response.SessionId = session.Id;
                var (accessToken, expiresAt) = _authenticationService.GenerateAccessToken(user, session.Id);
                response.AccessToken = accessToken;
                response.ExpiresAt = expiresAt;
                NotificationUserModel dataModel = new NotificationUserModel()
                {
                    UserId = user.Id.ToString(),
                    Content = "Chào mừng bạn đến tới highschoolvn, chúc bạn học được nhìu điều bổ ích!",
                    Title = "Chào mừng" + user.Username,
                };

                await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.NotificationUserCreated, user.Id.ToString(), dataModel);
                return new ResponseModel()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageUser.LoginSuccess,
                    Data = response
                };
            }

            return new ResponseModel()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.ServerError
            };
        }
    }
}
