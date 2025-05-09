using Application.Common.Models.AuthenModel;
using Application.Common.Models.Common;
using Application.Common.Ultils;
using Application.Constants;
using Application.Services.ServiceTask.Common;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Common.Messages;
using Domain.Common.UUID;
using Domain.Constants;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using SharedProject.Models;
using System.Net;

namespace Application.Features.Authen.v2.LoginWithGoogle
{
    public class LoginWithGoogleCommandHandler(IAuthenticationService authenticationService, IUnitOfWork unitOfWork,
            IMapper mapper, ICommonTask commonTask, IProducerService producerService) : IRequestHandler<LoginWithGoogleCommand, ResponseModel>
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ICommonTask _commonTask = commonTask;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
        {
            var result = await _authenticationService.ValidateGoogleToken2(request.AccessToken!, request.Email!);

            if (result.Status != HttpStatusCode.OK)
            {
                return result;
            }

            var user = await _unitOfWork.UserRepository.GetUserByUsernameOrEmail(request.Email);

            if (user == null)
            {
                var username = request.Email.ToSplitUsername();
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
                    Provider = ProviderConstant.GOOGLE,
                    ProfilePicture = request.Avatar ?? AvatarExtension.GetAvatar(username),
                    Fullname = request.FullName ?? username,
                };

                await _unitOfWork.UserRepository.AddAsync(user);
                NotificationUserModel dataModel = new NotificationUserModel()
                {
                    UserId = user.Id.ToString(),
                    Content = "Chào mừng bạn đến tới highschoolvn, chúc bạn học được nhìu điều bổ ích!",
                    Title = "Chào mừng" + user.Username,
                };

                _ = Task.Run(async () =>
                {
                    var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.NotificationUserCreated, user.Id.ToString(), dataModel);
                }, cancellationToken);
            } else if (user.DeletedAt != null)
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
