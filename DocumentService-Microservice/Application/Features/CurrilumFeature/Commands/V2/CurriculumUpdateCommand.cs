using Application.Common.Models.CurriculumModel;
using Application.Common.Models;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using System.Net;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CurrilumFeature.Commands.V2
{
	public class CurriculumUpdateCommand : IRequest<ResponseModel>
	{
		public required CurriculumUpdateRequestModel CurriculumUpdateRequestModel;
		public required Guid CurriculumId;
	}

	public class CurriculumUpdateCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CurriculumUpdateCommand, ResponseModel>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ResponseModel> Handle(CurriculumUpdateCommand request, CancellationToken cancellationToken)
		{
			var curriculum = await _unitOfWork.CurriculumRepository.GetCurriculumById(request.CurriculumId);
			if (curriculum == null)
			{
				return new ResponseModel(HttpStatusCode.NotFound, ResponseConstaints.CurriculumNotFound);
			}

			var listSubjectCurriculum = await _unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculumOfCurriculum(request.CurriculumId);
			if (listSubjectCurriculum == null || !listSubjectCurriculum.Any())
			{
				
			}

			await _unitOfWork.BeginTransactionAsync();

			try
			{
				foreach (var subjectCurriculum in listSubjectCurriculum)
				{
					var nameParts = subjectCurriculum.SubjectCurriculumName.Split(' ', 2);
					if (nameParts.Length > 0)
					{
						subjectCurriculum.SubjectCurriculumName = $"{nameParts[0]} {request.CurriculumUpdateRequestModel.CurriculumName}";
					}
					else
					{
						subjectCurriculum.SubjectCurriculumName = $"{subjectCurriculum.SubjectCurriculumName} {request.CurriculumUpdateRequestModel.CurriculumName}";
					}
					_unitOfWork.SubjectCurriculumRepository.Update(subjectCurriculum);
				}
				curriculum.CurriculumName = request.CurriculumUpdateRequestModel.CurriculumName;
				curriculum.ImageUrl = request.CurriculumUpdateRequestModel.ImageUrl ?? curriculum.ImageUrl;
				curriculum.IsExternal = request.CurriculumUpdateRequestModel.IsExternal;
				curriculum.UpdatedAt = DateTime.UtcNow;
				_unitOfWork.CurriculumRepository.Update(curriculum);

				var result = await _unitOfWork.SaveChangesAsync();
				if (result <= 0)
				{
					await _unitOfWork.RollbackTransactionAsync();
					return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.CurriculumUpdateFailed);
				}

				await _unitOfWork.CommitTransactionAsync();
				return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.CurriculumUpdated);
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				return new ResponseModel(HttpStatusCode.InternalServerError, $"Failed to update curriculum: {ex.Message}");
			}
		}
	}
}