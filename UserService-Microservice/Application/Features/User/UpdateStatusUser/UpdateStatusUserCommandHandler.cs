using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Enumerations;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.User.UpdateStatusUser
{
	public class UpdateStatusUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateStatusUserCommand, ResponseModel>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResponseModel> Handle(UpdateStatusUserCommand request, CancellationToken cancellationToken)
		{
			var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);

			if (user == null)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			}

			var status = EnumExtensions.ConvertToStatusValue(request.Status);

			if (status == null)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.BadRequest,
					Message = MessageCommon.InvalidStatus
				};
			}

            if ((int)status! == (int)AccountStatus.Deleted)
			{
				user.DeletedAt = DateTime.Now;
			}

            user.Status = status.ToString();

            _unitOfWork.UserRepository.Update(user);

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
				Message = MessageCommon.UpdateFailed
			};

		}
	}
}
