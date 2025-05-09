using Application.Common.Interfaces.ClaimInterface;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectFeature.Queries;

public record SubjectIdsQuery : IRequest<List<SubjectModel>>
{
    public List<Guid> SubjectIds;
}

public class SubjectIdsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claimInterface)
    : IRequestHandler<SubjectIdsQuery, List<SubjectModel>>
{
    public async Task<List<SubjectModel>> Handle(SubjectIdsQuery request, CancellationToken cancellationToken)
    {
        var listSubject = new List<SubjectModel>();

        var isStudent = claimInterface.GetCurrentUserId != Guid.Empty && claimInterface.GetRole.Contains("Student", StringComparison.OrdinalIgnoreCase);

        foreach (var id in request.SubjectIds)
        {
            var result = await unitOfWork.SubjectRepository.GetSubjectBySubjectId(id);

            //#region Added 19/10/2024 for Progress

            //try
            //{
            //    var numberOfEnrollment = await unitOfWork.EnrollmentRepository.CountAsync(filter: enroll => enroll.SubjectCurriculum.Subject.Id == id);

            //    result.NumberEnrollment = numberOfEnrollment;

            //    if (isStudent)
            //    {
            //        var enroll = await unitOfWork.EnrollmentRepository.Get(filter: enroll => enroll.SubjectCurriculum.Subject.Id == id && enroll.BaseUserId == claimInterface.GetCurrentUserId);

            //        if (enroll.Any())
            //        {
            //            result.IsEnroll = true;
            //        }
            //        else
            //        {
            //            result.EnrollmentProgress = null;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
            //#endregion
            if (result is not null)
            {
                listSubject.Add(result);
            }
            
        }

        listSubject = listSubject.OrderBy(x => x.Category).ToList();

        return listSubject;
    }
}