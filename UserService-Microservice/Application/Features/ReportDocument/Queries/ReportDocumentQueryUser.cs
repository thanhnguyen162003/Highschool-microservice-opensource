using Application.Common.Models.Common;
using Application.Common.Models.ReportDocumentModel;
using Application.Common.Models.UserModel;
using Domain.Common.Models;
using Domain.Common.Ultils;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.ReportDocument.Queries
{
	public record ReportDocumentQueryUser : IRequest<PagedList<ReportDocumentResponseModel>>
	{
		public int Page { get; init; } = 1; 
		public int EachPage { get; init; } = 12;
	}

	public class ReportDocumentQueryUserHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : IRequestHandler<ReportDocumentQueryUser, PagedList<ReportDocumentResponseModel>>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<PagedList<ReportDocumentResponseModel>> Handle(ReportDocumentQueryUser request, CancellationToken cancellationToken)
		{
			var userId = _httpContextAccessor.HttpContext!.User.GetUserIdFromToken();
			var reportDocumentsPaged = await _unitOfWork.ReportDocumentRepository.GetReportDocumentForUser(
				userId,
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
				CreatedAt = rd.CreatedAt,
				UserId = userId,
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