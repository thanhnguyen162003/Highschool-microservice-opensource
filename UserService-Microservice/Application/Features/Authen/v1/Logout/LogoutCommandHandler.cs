using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.Authen.v1.Logout
{
    public class LogoutCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : IRequestHandler<LogoutCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<ResponseModel> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext!.User.GetUserIdFromToken();
            var session = await _unitOfWork.SessionRepository.GetSessionUser(request.SessionId, userId);

            if (session != null)
            {
                session.IsRevoked = true;
                session.UpdatedAt = DateTime.Now;
                _unitOfWork.SessionRepository.Update(session);

                if (await _unitOfWork.SaveChangesAsync())
                {
                    return new ResponseModel()
                    {
                        Status = HttpStatusCode.InternalServerError,
                        Message = MessageUser.LogoutFailed
                    };
                }
            }

            return new ResponseModel
            {
                Status = HttpStatusCode.OK,
                Message = MessageUser.LogoutSuccess
            };
        }
    }
}
