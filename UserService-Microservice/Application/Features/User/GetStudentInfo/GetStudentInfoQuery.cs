using Application.Common.Models.Common;
using Application.Common.Models.UserModel;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.User.GetStudentInfo
{

    public class GetStudentInfoQuery : IRequest<ResponseModel>
    {

    }

    public class GetStudentInfoQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetStudentInfoQuery, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<ResponseModel> Handle(GetStudentInfoQuery request, CancellationToken cancellationToken)
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

            var baseUser = await _unitOfWork.UserRepository.GetUserByUserId(student.BaseUserId);

            var result = _mapper.Map<StudentCardResponseModel>(student);

            return new ResponseModel
            {
                Status = HttpStatusCode.OK,
                Message = MessageCommon.GetSuccess,
                Data = result
            };

        }
    }
}
