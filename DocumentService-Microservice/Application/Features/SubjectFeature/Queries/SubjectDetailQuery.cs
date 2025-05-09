using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Constants;
using Application.MaintainData.KafkaMessageModel;
using Dapr.Client;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.SubjectFeature.Queries;

public record SubjectDetailQuery : IRequest<SubjectModel>
{
    public Guid subjectId;
}

public class SubjectDetailQueryHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface,
    IProducerService producerService, DaprClient client, IClaimInterface claim)
    : IRequestHandler<SubjectDetailQuery, SubjectModel>
{
    public async Task<SubjectModel> Handle(SubjectDetailQuery request, CancellationToken cancellationToken)
    {
        var subject = await unitOfWork.SubjectRepository.GetSubjectBySubjectId(request.subjectId, cancellationToken);
        var userId = claim.GetCurrentUserId;
		if (subject is null)
		{
			return new SubjectModel();
		}
		var response = await client.InvokeMethodAsync<ResponseModel>(
							HttpMethod.Get,
							"user-sidecar",
							$"api/v1/dapr/subject-curriculum-user/subject/{request.subjectId}/user/{userId}"
						);
		if (response.Status == HttpStatusCode.Found)
		{
            subject.CurriculumIdUser = Guid.Parse(response.Data.ToString());
		}
		if (claimInterface.GetCurrentUserId != Guid.Empty)
        {
            _ = Task.Run(() =>
            {
                producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectViewUpdate, subject.Id.ToString(), subject.Id.ToString());
            }, cancellationToken);

            RecentViewModel recentView = new RecentViewModel()
            {
                UserId = claimInterface.GetCurrentUserId,
                IdDocument = subject.Id,
                SlugDocument = subject.Slug,
                TypeDocument = TypeDocumentConstaints.Subject,
                DocumentName = subject.SubjectName,
                Time = DateTime.UtcNow
            };
            _ = Task.Run(() =>
                producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecentViewCreated,
                claimInterface.GetCurrentUserId.ToString(), recentView), cancellationToken);
            
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