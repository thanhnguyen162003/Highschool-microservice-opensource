using Application.Common.Models.Common;
using Application.Common.Ultils;
using Domain.Common.Messages;
using Domain.Common.UUID;
using Domain.Constants;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.User.CreateAccount
{
	public class CreateAccountCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService) : IRequestHandler<CreateAccountCommand, ResponseModel>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<ResponseModel> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
		{
			_authenticationService.CreatePasswordHash(request.Password!, out var passwordHash, out var passwordSalt);

			var user = new BaseUser
			{
				Id = new UuidV7().Value,
				Username = request.Username,
				Email = request.Email,
				Bio = request.Bio,
				Fullname = request.Fullname,
				Password = passwordHash,
				PasswordSalt = passwordSalt,
				ProfilePicture = request.ProfilePicture ?? AvatarExtension.GetAvatar(request.Fullname!),
				CreatedAt = DateTime.Now,
				ProgressStage = ProgressStage.NewUser.ToString(),
				Provider = ProviderConstant.CREDENTIAL,
				RoleId = (int)RoleEnum.Moderator,
				Status = AccountStatus.Active.ToString(),
				Timezone = TimeZoneInfo.Utc.ToString()
			};

			await _unitOfWork.UserRepository.AddAsync(user);

			if (await _unitOfWork.SaveChangesAsync())
			{


				return new ResponseModel
				{
					Status = HttpStatusCode.Created,
					Message = MessageCommon.CreateSuccesfully,
					Data = user.Id
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
