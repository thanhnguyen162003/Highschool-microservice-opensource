using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;
using System.Net;

namespace Application.Features.User.GetUserMbti
{
	public class GetUserMbtiTypeCommand : IRequest<ResponseModel>
	{

	}

	public class GetUserMbtiTypeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, CareerMongoDatabaseContext context) : IRequestHandler<GetUserMbtiTypeCommand, ResponseModel>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
		private readonly CareerMongoDatabaseContext _context = context;

        public async Task<ResponseModel> Handle(GetUserMbtiTypeCommand request, CancellationToken cancellationToken)
		{
			var userId = _httpContextAccessor.HttpContext?.User.GetUserIdFromToken();
			var student = await _unitOfWork.StudentRepository.GetStudentByUserId(userId!.Value);

			if (student == null)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			}

			var result = student.MbtiType == null ? null : await _context.MBTITypeContents.Find(content => content.MBTIType == student.MbtiType).FirstOrDefaultAsync();

			return new ResponseModel
			{
				Status = HttpStatusCode.OK,
				Message = MessageCommon.GetSuccess,
				Data = result
			};

		}
	}
}
