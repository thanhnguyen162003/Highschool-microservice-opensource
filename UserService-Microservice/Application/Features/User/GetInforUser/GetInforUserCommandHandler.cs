using Application.Common.Models.Common;
using Application.Common.Models.UserModel;
using Domain.Common.Messages;
using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.User.GetInforUser
{
	public class GetInforUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetInforUserCommand, ResponseModel>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(GetInforUserCommand request, CancellationToken cancellationToken)
		{
			var baseUser = await _unitOfWork.UserRepository.GetUserByUsername(request.UserName);

			if (baseUser == null)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			} else if (baseUser.RoleId == (int)RoleEnum.Student)
			{
				return await GetStudentInfo(baseUser!);
			} else if (baseUser.RoleId == (int)RoleEnum.Teacher)
			{
				return await GetTeacherInfo(baseUser!);
			}

			return new ResponseModel
			{
				Status = HttpStatusCode.OK,
				Message = MessageCommon.GetSuccess,
				Data = _mapper.Map<BaseUserInforResponseModel>(baseUser)
			};

		}

		private async Task<ResponseModel> GetStudentInfo(BaseUser user)
		{
			var student = await _unitOfWork.StudentRepository.GetStudentByUserId(user.Id);

			if (student == null)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.OK,
					Message = MessageUser.MissingProfileStudent,
					Data = _mapper.Map<BaseUserInforResponseModel>(user)
                };
			}

            var userResponse = _mapper.Map<StudentInfoResponseModel>(user);

            return new ResponseModel()
			{
				Status = HttpStatusCode.OK,
				Message = MessageCommon.GetSuccess,
				Data = _mapper.Map(student, userResponse)
			};
		}

		private async Task<ResponseModel> GetTeacherInfo(BaseUser user)
		{
			var teacher = await _unitOfWork.TeacherRepository.GetTeacherByUserId(user.Id);

			if (teacher == null)
			{
                return new ResponseModel
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageUser.MissingProfileTeacher,
                    Data = _mapper.Map<BaseUserInforResponseModel>(user)
                };
            }

			var userResponse = _mapper.Map<TeacherInfoResponseModel>(user);

			return new ResponseModel()
			{
				Status = HttpStatusCode.OK,
				Message = MessageCommon.GetSuccess,
				Data = _mapper.Map(teacher, userResponse)
			};
		}
	}
}
