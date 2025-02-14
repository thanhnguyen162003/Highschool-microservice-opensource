using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.SubjectFeature.Queries;

public record SubjectDetailSlugQuery : IRequest<SubjectModel>
{
    public string slug;
}

public class SubjectDetailSlugQueryHandler(IUnitOfWork unitOfWork, IProducerService producerService, IClaimInterface claimInterface,
    ILogger<SubjectDetailSlugQueryHandler> logger, IMapper mapper)
    : IRequestHandler<SubjectDetailSlugQuery, SubjectModel>
{
    public async Task<SubjectModel> Handle(SubjectDetailSlugQuery request, CancellationToken cancellationToken)
    {
        var subject = await unitOfWork.SubjectRepository.GetSubjectBySubjectSlug(request.slug, cancellationToken);
        
        if (claimInterface.GetCurrentUserId != Guid.Empty)
        {
            UserAnalyseMessageModel dataModel = new UserAnalyseMessageModel()
            {
                UserId = claimInterface.GetCurrentUserId,
                SubjectId = subject.Id
            };
            RecentViewModel recentView = new RecentViewModel()
            {
                UserId = claimInterface.GetCurrentUserId,
                IdDocument = subject.Id,
                SlugDocument = subject.Slug,
                TypeDocument = TypeDocumentConstaints.Subject,
                DocumentName = subject.SubjectName,
                Time = DateTime.Now
            };
            
            _ = Task.Run(() =>
            {
                producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectViewUpdate, subject.Id.ToString(), subject.Id.ToString());
            }, cancellationToken);
            
            _ = Task.Run(() =>
            {
                producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.UserAnalyseData, claimInterface.GetCurrentUserId.ToString(), dataModel);
            }, cancellationToken);
            _ = Task.Run(() =>
                producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecentViewCreated,
                    claimInterface.GetCurrentUserId.ToString(), recentView), cancellationToken);

            //var isStudent = claimInterface.GetCurrentUserId != Guid.Empty && claimInterface.GetRole.Contains("Student", StringComparison.OrdinalIgnoreCase);

            //try
            //{
            //    var numberOfEnrollment = await unitOfWork.EnrollmentRepository.CountAsync(filter: enroll => enroll.SubjectCurriculum.Subject.Id == subject.Id);

            //    subject.NumberEnrollment = numberOfEnrollment;

            //    if (isStudent)
            //    {
            //        var enroll = await unitOfWork.EnrollmentRepository.Get(filter: enroll => enroll.SubjectCurriculum.Subject.Id == subject.Id && enroll.BaseUserId == claimInterface.GetCurrentUserId);

            //        if (enroll.Any())
            //        {
            //            subject.IsEnroll = true;

            //            var progresses = await unitOfWork.EnrollmentProgressRepository.Get(filter: progress => progress.EnrollmentId == enroll.FirstOrDefault()!.Id
            //                                                                                                && progress.Lesson.DeletedAt == null
            //                                                                                                && progress.Lesson.Chapter.DeletedAt == null
            //                                                                                                && progress.Lesson.Chapter.SubjectCurriculum.Subject.DeletedAt == null,
            //                                                                               includeProperties: [progress => progress.Lesson,
            //                                                                                            progress => progress.Lesson.Chapter,
            //                                                                                            progress => progress.Lesson.Chapter.SubjectCurriculum.Subject]);

            //            subject.EnrollmentProgress = new EnrollmentProgressModel();

            //            if (progresses.Any())
            //            {
            //                progresses = progresses.OrderByDescending(progress => progress.UpdatedAt);

            //                subject.EnrollmentProgress.LastViewedLesson = mapper.Map<EntitySimpleModel>(progresses.FirstOrDefault()!.Lesson);

            //                subject.EnrollmentProgress.LastViewedChapter = mapper.Map<EntitySimpleModel>(progresses.FirstOrDefault()!.Lesson.Chapter);

            //                var allSubjectLessonCount = await unitOfWork.LessonRepository.CountAsync(filter: lesson => lesson.Chapter.SubjectCurriculum.SubjectId == subject.Id
            //                                                                                                        && lesson.DeletedAt == null
            //                                                                                                        && lesson.Chapter.DeletedAt == null
            //                                                                                                        && lesson.Chapter.SubjectCurriculum.Subject.DeletedAt == null);

            //                subject.EnrollmentProgress.SubjectProgressPercent = (float)Math.Round((double)progresses.Count() / allSubjectLessonCount, 2) * 100;
            //            }
            //            else
            //            {
            //                subject.EnrollmentProgress.SubjectProgressPercent = 0;
            //                subject.EnrollmentProgress.LastViewedChapter = null;
            //                subject.EnrollmentProgress.LastViewedLesson = null;
            //            }
            //        }
            //        else
            //        {
            //            subject.EnrollmentProgress = null;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
            var userLike = await unitOfWork.UserLikeRepository.GetUserLikeSubjectAsync(claimInterface.GetCurrentUserId, subject.Id);
            if (userLike != null)
            {
                if (userLike.SubjectId == subject.Id)
                {
                    subject.IsLike = true;
                }
                else
                {
                    subject.IsLike = false;
                }
            }
        }
        
        return subject;
    }
}