using Application.Common.Models.Common;
using Domain.Common.Messages;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.User.UpdateTeacher
{
	public class UpdateTeacherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateTeacherCommand, ResponseModel>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
		{
			var teacher = await _unitOfWork.TeacherRepository.GetTeacherByUserId(request.BaseUserId);

			if (teacher == null)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			}

			var newTeacher = _mapper.Map(request, teacher);

			_unitOfWork.TeacherRepository.Update(newTeacher);

			if (await _unitOfWork.SaveChangesAsync())
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.OK,
					Message = MessageCommon.UpdateSuccesfully
				};
			}

			return new ResponseModel
			{
				Status = HttpStatusCode.InternalServerError,
				Message = MessageCommon.ServerError
			};
		}
	}
}
