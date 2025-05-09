using Application.Common.Models.Common;
using Application.Common.Models.UserModel;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.User.GetPersonalityTestStatus
{
    public class GetPersonalityTestStatusQuery : IRequest<ResponseModel>
    {

    }

    public class GetPersonalityTestStatusQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetPersonalityTestStatusQuery, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<ResponseModel> Handle(GetPersonalityTestStatusQuery request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserIdFromToken();
            var student = await _unitOfWork.StudentRepository.GetStudentByUserId(userId!.Value);

            if (student == null)
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageUser.UserIsNotStudent
                };
            }

            return new ResponseModel
            {
                Status = HttpStatusCode.OK,
                Message = MessageCommon.GetSuccess,
                Data = new PersonalityTestStatusResponseModel()
                {
                    IsHollandDone = !string.IsNullOrEmpty(student.HollandType),
                    IsMBTIDone = student.MbtiType != null
                }
            };

        }
    }
}
