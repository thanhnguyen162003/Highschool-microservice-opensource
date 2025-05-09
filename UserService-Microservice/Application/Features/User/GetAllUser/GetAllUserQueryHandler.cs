using Application.Common.Models.UserModel;
using Domain.Common;
using Domain.Common.Models;
using Domain.Common.Ultils;
using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.User.GetAllUser
{
	public class GetAllUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAllUserQuery, (object, Metadata)>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;

        public async Task<(object, Metadata)> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
		{

			var roleId = (int)request.RoleName.ConvertToValue<RoleEnum>()!;

			if (roleId == (int)RoleEnum.Student)
			{
				var result = await _unitOfWork.UserRepository.GetAllUser(request.Page, request.EachPage, roleId, request.Search, request.Status);

				var students = _mapper.Map<PagedList<StudentResponse>>(result);

				return (students, students.Metadata);
			} else if (roleId == (int)RoleEnum.Teacher) {
				var result = await _unitOfWork.UserRepository.GetAllUser(request.Page, request.EachPage, roleId, request.Search, request.Status);

				var teachers = _mapper.Map<PagedList<TeacherResponse>>(result);

				return (teachers, teachers.Metadata);
			} else if (EnumExtensions.IsModerator(request.RoleName))
			{
				var result = await _unitOfWork.UserRepository.GetAllModerator(request.Page, request.EachPage, request.Search, request.Status);

				var moderators = _mapper.Map<PagedList<BaseUserResponse>>(result);

				return (moderators, moderators.Metadata);
			}


			return (new PagedList<object>(), new Metadata());
		}
	}
}
