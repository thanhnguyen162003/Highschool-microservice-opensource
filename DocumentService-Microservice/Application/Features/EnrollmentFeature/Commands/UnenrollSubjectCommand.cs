using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Services.CacheService.Intefaces;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.EnrollmentFeature.Commands
{
    public record UnenrollSubjectCommand : IRequest<ResponseModel>
    {
        public Guid CurriculumId { get; set; }
        public Guid SubjectId { get; set; }
    }

    public class UnenrollSubjectCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface,
        ICleanCacheService cleanCacheService) : IRequestHandler<UnenrollSubjectCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(UnenrollSubjectCommand request, CancellationToken cancellationToken)
        {
            var userId = claimInterface?.GetCurrentUserId;
            var userRole = claimInterface?.GetRole;

            if (userId.HasValue)
            {
                await unitOfWork.BeginTransactionAsync();

                var subjectCurriculumExist = await unitOfWork.SubjectCurriculumRepository.Get(sc => sc.SubjectId == request.SubjectId && sc.CurriculumId == request.CurriculumId);

                if (subjectCurriculumExist == null)
                {
                    await unitOfWork.RollbackTransactionAsync();

                    return new ResponseModel
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Môn học và chương trình học không hợp lệ"
                    };
                }

                var subjectCurriculum = subjectCurriculumExist.First();

                var enrollmentInfo = await unitOfWork.EnrollmentRepository.Get(filter: enrollment => enrollment.SubjectCurriculumId == subjectCurriculum.Id && enrollment.BaseUserId == userId);

                if (enrollmentInfo.Any())
                {
                    unitOfWork.EnrollmentRepository.Delete(enrollmentInfo.First());
                }
                await cleanCacheService.ClearRelatedCacheSpecificUser(userId);
                await unitOfWork.CommitTransactionAsync();
                
                return new ResponseModel()
                {
                    Status = HttpStatusCode.NoContent,
                    Message = "Thành công",
                };
            }
            else
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Bạn chưa đăng nhập"
                };
            }
        }
    }
}
