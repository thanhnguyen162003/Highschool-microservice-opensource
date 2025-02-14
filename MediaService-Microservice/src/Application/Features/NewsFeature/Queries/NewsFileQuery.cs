using Application.Common.Models;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.NewsModel;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.NewsFeature.Queries;

public record NewsFileQuery : IRequest<List<NewsFileResponseModel>>
{
    public Guid NewsId;
}
public class NewsFileQueryHandler(MediaDbContext dbContext, IMapper mapper) : IRequestHandler<NewsFileQuery, List<NewsFileResponseModel>>
{
    public async Task<List<NewsFileResponseModel>> Handle(NewsFileQuery request, CancellationToken cancellationToken)
    {
        var listTheory = await dbContext.NewsFiles
            .Find(theoryFile => theoryFile.NewsId.Equals(request.NewsId)).ToListAsync();
        return mapper.Map<List<NewsFileResponseModel>>(listTheory);
    }
}
