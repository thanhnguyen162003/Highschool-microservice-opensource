using Application.Common.Models.UniversityModel;
using Domain.Common.Models;
using Infrastructure.CustomEntities;
using Infrastructure.Data;
using Infrastructure.QueryFilters;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Features.University.Queries
{
    public record GetUniversitiesName : IRequest<PagedList<UniversityNameResponseModel>>
    {
        public UniversityQueryFilter QueryFilter { get; set; }
    }

    public class GetUniversitiesNameHandler(CareerMongoDatabaseContext context, IMapper mapper, IOptions<PaginationOptions> paginationOptions) : IRequestHandler<GetUniversitiesName, PagedList<UniversityNameResponseModel>>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

        public async Task<PagedList<UniversityNameResponseModel>> Handle(GetUniversitiesName request, CancellationToken cancellationToken)
        {
            request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
            request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;

            FilterDefinition<Domain.MongoEntities.University> filter;

            if (!string.IsNullOrEmpty(request.QueryFilter.Search))
            {
                var searchRegex = new BsonRegularExpression($".*{request.QueryFilter.Search}.*", "i");
                filter = Builders<Domain.MongoEntities.University>.Filter.Or(
                    Builders<Domain.MongoEntities.University>.Filter.Regex(x => x.name, searchRegex),
                    Builders<Domain.MongoEntities.University>.Filter.Regex(x => x.unicode, searchRegex)
                );
            }
            else
            {
                filter = Builders<Domain.MongoEntities.University>.Filter.Empty;
            }
            var result = new List<Domain.MongoEntities.University>();
            if (request.QueryFilter.PageNumber == -1)
            {
                result = await _context.Universities.Find(filter)
                .ToListAsync(cancellationToken);
            }
            else
            {
                result = await _context.Universities.Find(filter)
                .Skip(request.QueryFilter.PageSize * (request.QueryFilter.PageNumber - 1))
                .Limit(request.QueryFilter.PageSize)
                .ToListAsync(cancellationToken);
            }
            var totalCount = await _context.Universities.CountDocumentsAsync(filter);

            if (!result.Any())
            {
                return new PagedList<UniversityNameResponseModel>(new List<UniversityNameResponseModel>(), 0, 0, 0);
            }
            var mappedResults = _mapper.Map<List<UniversityNameResponseModel>>(result);
            return new PagedList<UniversityNameResponseModel>(
                mappedResults,
                Convert.ToInt32(totalCount),
                request.QueryFilter.PageNumber,
                request.QueryFilter.PageSize
            );
        }
    }

}
