using Application.Common.Models.DaprModel.Academic;
using Application.Common.Models.DaprModel.User;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.Dapr.Academic
{
    public record DaprGetUsersAcademic : IRequest<AcademicUserResponseDapr>
    {
        public required IEnumerable<string> Email { get; set; }
    }
    public class DaprGetUsersAcademicHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetUsersAcademic, AcademicUserResponseDapr>
    {
        public async Task<AcademicUserResponseDapr> Handle(DaprGetUsersAcademic request, CancellationToken cancellationToken)
        {
            var academicData = await unitOfWork.UserRepository.GetAllByEmails(request.Email);

            var users = mapper.Map<IEnumerable<AcademicUserResponse>>(academicData);

            return new AcademicUserResponseDapr()
            {
                Users = users.Select(user => new AcademicUserObjectDapr()
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Role = user.Role,
                    Avatar = user.Avatar,
                    Fullname = user.FullName,
                    Email = user.Email
                }).ToList()
            };
        }
    }
}
