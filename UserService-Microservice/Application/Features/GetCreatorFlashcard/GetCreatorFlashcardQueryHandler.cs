using Application.Common.Models.Common;
using Application.Common.Models.External;
using Domain.Common.Messages;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.GetCreatorFlashcard
{
	public class GetCreatorFlashcardQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCreatorFlashcardQuery, ResponseModel>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(GetCreatorFlashcardQuery request, CancellationToken cancellationToken)
		{
			var user = await _unitOfWork.UserRepository.GetDetailUser(request.UserId);

			if (user == null)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			}

			return new ResponseModel()
			{
				Status = HttpStatusCode.OK,
				Message = MessageCommon.GetSuccess,
				Data = _mapper.Map<AuthorFlashcardResponse>(user)
			};

		}
	}
}
