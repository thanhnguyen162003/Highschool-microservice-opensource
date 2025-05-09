using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Constants;
using Application.Features.MasterSubjectFeature.Commands;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.CurrilumFeature.Commands
{
	public record CurriculumDeleteCommand : IRequest<ResponseModel>
	{
		public Guid Id { get; init; }
	}

	public class CurriculumDeleteCommandHandler(IUnitOfWork unitOfWork)
		: IRequestHandler<CurriculumDeleteCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(CurriculumDeleteCommand request, CancellationToken cancellationToken)
		{
			var result = await unitOfWork.CurriculumRepository.DeleteCurriculum(request.Id);
			if (result is true)
			{
				return new ResponseModel(HttpStatusCode.Created, "Ok");
			}
			return new ResponseModel(HttpStatusCode.BadRequest, "Not good :<");
		}
	}
}
