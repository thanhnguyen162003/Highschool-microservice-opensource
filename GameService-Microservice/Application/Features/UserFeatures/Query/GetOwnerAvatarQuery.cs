using Application.Services.Authentication;
using Infrastructure;
using MediatR;

namespace Application.Features.UserFeatures.Query
{
    public class GetOwnerAvatarQuery : IRequest<IEnumerable<Guid>>
    {
    }

    public class GetOwnerAvatarQueryHandler : IRequestHandler<GetOwnerAvatarQuery, IEnumerable<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;

        public GetOwnerAvatarQueryHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
        }

        public async Task<IEnumerable<Guid>> Handle(GetOwnerAvatarQuery request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.GetUserId();

            var user = await _unitOfWork.UserRepository.GetById(userId);

            return user!.OwnerAvatar ?? new List<Guid>();
        }
    }
}
