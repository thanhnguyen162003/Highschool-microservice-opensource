using Application.Common.Models.AuthenModel;
using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Common.Security;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;

namespace Application.Features.Authen.v2.RefreshToken
{
    public class RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IOptions<JWTSetting> options, IAuthenticationService authenticationService) : IRequestHandler<RefreshTokenCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly JWTSetting _jwtSettings = options.Value;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<ResponseModel> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.SessionRepository.GetSession(request.RefreshToken, request.SessionId);

            if (session == null)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageUser.RefreshTokenFailed
                };
            } else if(session.IsRevoked)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.Unauthorized,
                    Message = MessageUser.TokenExpired
                };
            }

            var timespan = DateTime.Now - session.ExpiredAt;

            if (timespan.TotalDays / 30 > _jwtSettings.RefreshTokenExpiry)
            {
                timespan = DateTime.Now - session.UpdatedAt;
                if (timespan.TotalDays / 30 > _jwtSettings.RefreshTokenExpiry)
                {
                    session.IsRevoked = true;
                    session.UpdatedAt = DateTime.Now;
                    _unitOfWork.SessionRepository.Update(session);
                    await _unitOfWork.SaveChangesAsync();
                    return new ResponseModel()
                    {
                        Status = HttpStatusCode.Unauthorized,
                        Message = MessageUser.TokenExpired
                    };
                }

                session.RefreshToken = _authenticationService.GenerateRefreshToken()!;
                session.ExpiredAt = DateTime.Now.AddMonths((int)_jwtSettings.RefreshTokenExpiry);
            }

            session.UpdatedAt = DateTime.Now;
            _unitOfWork.SessionRepository.Update(session);

            if (await _unitOfWork.SaveChangesAsync())
            {
                var (accessToken, expiresAt) = _authenticationService.GenerateAccessToken(session.User, session.Id);
                return new ResponseModel()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageUser.RefreshTokenSuccess,
                    Data = new RefreshTokenResponse()
                    {
                        AccessToken = accessToken,
                        RefreshToken = session.RefreshToken,
                        ExpiresAt = expiresAt,
                    }
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
