using Application.Common.Models.ReportModel;
using Domain.Common.Interfaces.ClaimInterface;
using Domain.Common.Models;
using Domain.Common.Ultils;
using Domain.Enumerations;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.User.GetReport
{
	public class GetReportCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface, IMapper mapper) : IRequestHandler<GetReportCommand, PagedList<ReportResponseModel>>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;
		private readonly IClaimInterface _claimInterface = claimInterface;

        public async Task<PagedList<ReportResponseModel>> Handle(GetReportCommand request, CancellationToken cancellationToken)
		{
			var roleId = int.Parse(_claimInterface.GetRole);
			var userId = _claimInterface.GetCurrentUserId;

			if (roleId == (int)RoleEnum.Student || roleId == (int)RoleEnum.Teacher)
			{
				var reports = await _unitOfWork.ReportRepository.GetReport(request.Page, request.EachPage, userId, request.Status, request.StartDate, request.EndDate, request.ReportId, request.IsAscending);

				return _mapper.Map<PagedList<ReportResponseModel>>(reports);
			} else
			{
				var reports = await _unitOfWork.ReportRepository.GetReport(request.Page, request.EachPage, request.Status, request.StartDate, request.EndDate, request.ReportId, request.IsAscending);

				return _mapper.Map<PagedList<ReportResponseModel>>(reports);
			}

		}

	}
}
