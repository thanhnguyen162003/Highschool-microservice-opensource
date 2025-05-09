using Application.Common.Models.ReportDocumentModel;
using Application.Common.Models.UserModel;
using Domain.Common.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.ReportDocument.Queries
{
	public record ReportDocumentQueryAdmin : IRequest<PagedList<ReportDocumentResponseModel>>
	{
		public int Page { get; init; } = 1;
		public int EachPage { get; init; } = 12;
	}

	public class ReportDocumentQueryAdminHandler(IUnitOfWork unitOfWork) : IRequestHandler<ReportDocumentQueryAdmin, PagedList<ReportDocumentResponseModel>>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<PagedList<ReportDocumentResponseModel>> Handle(ReportDocumentQueryAdmin request, CancellationToken cancellationToken)
		{
			var reportDocumentsPaged = await _unitOfWork.ReportDocumentRepository.GetReportDocumentForAdmin(
				request.Page,
				request.EachPage,
				cancellationToken);

			var responseData = reportDocumentsPaged.Items.Select(rd => new ReportDocumentResponseModel
			{
				Id = rd.Id,
				ReportTitle = rd.ReportTitle,
				ReportContent = rd.ReportContent,
				Status = rd.Status,
				DocumentId = rd.DocumentId,
				ReportType = rd.ReportType.ToString(),
				UserId = rd.UserId,
				CreatedAt = rd.CreatedAt,
				User = new BaseUserResponse
				{
					Id = rd.User.Id,
					Username = rd.User.Username,
					Email = rd.User.Email,
					Fullname = rd.User.Fullname,
					RoleName = rd.User.Role.RoleName,
					Status = rd.User.Status,
					ProfilePicture = rd.User.ProfilePicture,
					CreatedAt = rd.User.CreatedAt
				}
			}).ToList();

			return new PagedList<ReportDocumentResponseModel>
			{
				Items = responseData,
				TotalPages = reportDocumentsPaged.TotalPages,
				CurrentPage = reportDocumentsPaged.CurrentPage,
				EachPage = reportDocumentsPaged.EachPage,
				TotalItems = reportDocumentsPaged.TotalItems
			};
		}
	}
}