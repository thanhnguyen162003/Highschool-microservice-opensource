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

public record SubjectDetailSlugQuery : IRequest<SubjectModel>
{
    public required string slug;
}

public class SubjectDetailSlugQueryHandler(IUnitOfWork unitOfWork, IProducerService producerService, IClaimInterface claimInterface,
    DaprClient client, IClaimInterface claim)
    : IRequestHandler<SubjectDetailSlugQuery, SubjectModel>
{
    public async Task<SubjectModel> Handle(SubjectDetailSlugQuery request, CancellationToken cancellationToken)
    {
        var subject = await unitOfWork.SubjectRepository.GetSubjectBySubjectSlug(request.slug, cancellationToken);
        if(subject is null)
        {
            return new SubjectModel();
        }
		var userId = claim.GetCurrentUserId;
		var response = await client.InvokeMethodAsync<ResponseModel>(
		                    HttpMethod.Get,
		                    "user-sidecar",
							$"api/v1/dapr/subject-curriculum-user/subject/{subject.Id}/user/{userId}"
						);
		if (response.Status == HttpStatusCode.Found)
		{
			subject.CurriculumIdUser = Guid.Parse(response.Data.ToString());
		}
		if (claimInterface.GetCurrentUserId != Guid.Empty)
        {
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
            {
                producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectViewUpdate, subject.Id.ToString(), subject.Id.ToString());
            }, cancellationToken);
            
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