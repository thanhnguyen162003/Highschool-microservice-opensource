using Application.Common.Models.UserModel;
using Domain.Common.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.User.GetAuthor
{
    public class GetAuthorCommand : IRequest<IEnumerable<BaseUserResponse>>
    {
        public IEnumerable<string> UserIds { get; set; } = new List<string>();
    }

    public class GetAuthorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAuthorCommand, IEnumerable<BaseUserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<BaseUserResponse>> Handle(GetAuthorCommand request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.UserRepository.GetAll(request.UserIds);
            var userResponses = _mapper.Map<IEnumerable<BaseUserResponse>>(users);

            return userResponses;

        }
    }

}
