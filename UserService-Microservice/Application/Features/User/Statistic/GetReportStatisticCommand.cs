using Application.Common.Models.Common;
using Application.Common.Models.ReportModel;
using Application.Common.Models.UserModel;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Enumerations;
using Domain.MongoEntities;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Net;
using ZstdSharp.Unsafe;

namespace Application.Features.User.Statistic
{
    public class GetReportStatisticCommand : IRequest<ReportAmountResponseModel>
    {
        public string Type { get; set; }
    }

    public class GetReportStatisticCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetReportStatisticCommand, ReportAmountResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ReportAmountResponseModel> Handle(GetReportStatisticCommand request, CancellationToken cancellationToken)
        {
            if (request.Type.ToLower().Equals("all"))
            {
                var total = await _unitOfWork.ReportDocumentRepository.GetTotalReportCount();
                total += await _unitOfWork.ReportRepository.GetTotalReportAmount();
                var thisMonth = await _unitOfWork.ReportRepository.GetTotalReportThisMonth();
                thisMonth += await _unitOfWork.ReportDocumentRepository.GetReportThisMonth();
                var lastMonth = await _unitOfWork.ReportRepository.GetTotalReportLastMonth();
                lastMonth += await _unitOfWork.ReportDocumentRepository.GetReportLastMonth();
                double percent = 0;
                if (lastMonth == 0)
                {
                    percent = (thisMonth - lastMonth) * 100 / 1;
                }
                else
                {
                    percent = (thisMonth - lastMonth) * 100 / lastMonth;
                }
                return new ReportAmountResponseModel()
                {
                    TotalReport = total,
                    ThisMonthReport = thisMonth,
                    IncreaseReportPercent = percent,
                };
            }
            else if (request.Type.Equals(ReportType.Flashcard.ToString()))
            {
                var count = await _unitOfWork.ReportDocumentRepository.GetTotalFlashcardReport();
                return new ReportAmountResponseModel()
                {
                    TotalReportType = count
                };
            }
            else if (request.Type.Equals(ReportType.FlashcardContent.ToString()))
            {
                var count = await _unitOfWork.ReportDocumentRepository.GetTotalFlashcardContentReport();
                return new ReportAmountResponseModel()
                {
                    TotalReportType = count
                };
            }
            else if (request.Type.Equals(ReportType.Comment.ToString()))
            {
                var count = await _unitOfWork.ReportDocumentRepository.GetTotalCommentReport();
                return new ReportAmountResponseModel()
                {
                    TotalReportType = count
                };
            }
            else if (request.Type.Equals(ReportType.Subject.ToString()))
            {
                var count = await _unitOfWork.ReportDocumentRepository.GetTotalSubjectReport();
                return new ReportAmountResponseModel()
                {
                    TotalReportType = count
                };
            }
            else if (request.Type.Equals(ReportType.Chapter.ToString()))
            {
                var count = await _unitOfWork.ReportDocumentRepository.GetTotalChapterReport();
                return new ReportAmountResponseModel()
                {
                    TotalReportType = count
                };
            }
            else if (request.Type.Equals(ReportType.Lesson.ToString()))
            {
                var count = await _unitOfWork.ReportDocumentRepository.GetTotalLessonReport();
                return new ReportAmountResponseModel()
                {
                    TotalReportType = count
                };
            }
            else if (request.Type.Equals(ReportType.Document.ToString()))
            {
                var count = await _unitOfWork.ReportDocumentRepository.GetTotalDocumentReport();
                return new ReportAmountResponseModel()
                {
                    TotalReportType = count
                };
            }
            else if (request.Type.Equals(ReportType.Quiz.ToString()))
            {
                var count = await _unitOfWork.ReportDocumentRepository.GetTotalQuizReport();
                return new ReportAmountResponseModel()
                {
                    TotalReportType = count
                };
            }
            else if (request.Type.Equals(ReportType.Test.ToString()))
            {
                var count = await _unitOfWork.ReportDocumentRepository.GetTotalTestReport();
                return new ReportAmountResponseModel()
                {
                    TotalReportType = count
                };
            }
            else
            {
                return new ReportAmountResponseModel();
            }
        }
    }
}
