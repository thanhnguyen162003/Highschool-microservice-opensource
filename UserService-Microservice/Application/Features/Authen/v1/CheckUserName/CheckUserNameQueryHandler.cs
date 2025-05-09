using Domain.Common.Ultils;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.Authen.v1.CheckUserName
{
    public class CheckUserNameQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : IRequestHandler<CheckUserNameQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<bool> Handle(CheckUserNameQuery request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext!.User.GetUserIdFromToken();
            var isExistUserName = await _unitOfWork.UserRepository.IsExistUserName(request.UserName, userId);

            return isExistUserName;

        }
    }
}
