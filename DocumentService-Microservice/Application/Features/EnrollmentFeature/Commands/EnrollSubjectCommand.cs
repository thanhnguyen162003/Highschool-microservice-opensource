using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.UUID;
using Application.Constants;
using Application.Services.CacheService.Intefaces;
using Infrastructure.Repositories.Interfaces;
using SharedProjects.ConsumeModel;
using System.Net;

namespace Application.Features.EnrollmentFeature.Commands
{
    public record EnrollSubjectCommand : IRequest<ResponseModel>
    {
        public Guid CurriculumId { get; set; }
        public Guid SubjectId { get; set; }
    }

    public class EnrollSubjectCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface,
        ICleanCacheService cleanCacheService, IProducerService producerService) : IRequestHandler<EnrollSubjectCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(EnrollSubjectCommand request, CancellationToken cancellationToken)
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

                if (!enrollmentInfo.Any())
                {
                    var newEnrollment = new Domain.Entities.Enrollment()
                    {
                        Id = new UuidV7().Value,
                        BaseUserId = userId.Value,
                        SubjectCurriculumId = subjectCurriculum.Id,
                    };

                    await unitOfWork.EnrollmentRepository.InsertAsync(newEnrollment);
                }

                await unitOfWork.CommitTransactionAsync();
                await cleanCacheService.ClearRelatedCacheSpecificUser(userId);
                NotificationUserModel dataModel = new NotificationUserModel()
                {
                    UserId = userId.ToString(),
                    Content = "Chúc bạn học được nhìu điều bổ ích!",
                    Title = "Bạn đã tham gia khoá học " + subjectCurriculum.SubjectCurriculumName,
                };
                var result = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.NotificationUserCreated, userId.ToString(), dataModel);
              
                return new ResponseModel()
                {
                    Status = HttpStatusCode.Created,
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
