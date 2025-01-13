using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.UUID;
using Application.Services.CacheService.Intefaces;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.EnrollmentProcessFeature.Commands
{
    public record AddProgressCommand : IRequest<ResponseModel>
    {
        public Guid LessonId { get; set; }
    }

    public class AddProgressCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface, ICacheOption cacheOption) : IRequestHandler<AddProgressCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(AddProgressCommand request, CancellationToken cancellationToken)
        {
            var userId = claimInterface?.GetCurrentUserId;
            var userRole = claimInterface?.GetRole;
            var lesson = await unitOfWork.LessonRepository.GetById(request.LessonId);

            if (lesson == null)
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Không tìm thấy bài học"
                };
            }

            var isEnroll = await unitOfWork.EnrollmentRepository.Get(filter: enroll => enroll.SubjectCurriculumId == lesson.Chapter.SubjectCurriculum.Id
                                                                                    && enroll.BaseUserId == userId);

            if (isEnroll.Any())
            {
                var enrollmentId = isEnroll.First().Id;
                var enrollmentProgress = await unitOfWork.EnrollmentProgressRepository.Get(filter: progress => progress.EnrollmentId == enrollmentId
                                                                                                                && progress.LessonId == request.LessonId);

                if (!enrollmentProgress.Any())
                {
                    var newProgress = new EnrollmentProgress()
                    {
                        Id = new UuidV7().Value,
                        EnrollmentId = isEnroll.First().Id,
                        LessonId = request.LessonId,
                    };

                    await unitOfWork.EnrollmentProgressRepository.InsertAsync(newProgress);

                    await unitOfWork.SaveChangesAsync();

                    return new ResponseModel
                    {
                        Status = HttpStatusCode.OK,
                        Message = "Quá trình học tập lưu thành công"
                    };
                }

                return new ResponseModel
                {
                    Status = HttpStatusCode.OK,
                    Message = "Quá trình học tập đã được lưu."
                };
            }
            else
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Bạn chưa đăng ký học"
                };
            }
        }
    }
}
