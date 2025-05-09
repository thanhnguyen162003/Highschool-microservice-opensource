using Application.Common.Models.OccupationModel;
using Domain.Common.Models;
using Infrastructure.CustomEntities;
using Infrastructure.Data;
using Infrastructure.QueryFilters;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Features.Occupation.Queries
{
    public record GetOccupations : IRequest<PagedList<Domain.MongoEntities.Occupation>>
    {
        public OccupationQueryFilter QueryFilter { get; set; }
    }

    public class GetOccupationsHandler(CareerMongoDatabaseContext context, IMapper mapper, IOptions<PaginationOptions> paginationOptions) : IRequestHandler<GetOccupations, PagedList<Domain.MongoEntities.Occupation>>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

        public async Task<PagedList<Domain.MongoEntities.Occupation>> Handle(GetOccupations request, CancellationToken cancellationToken)
        {
            request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
            request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;

            FilterDefinition<Domain.MongoEntities.Occupation> filter;

            if (!string.IsNullOrEmpty(request.QueryFilter.Search))
            {
                var searchRegex = new BsonRegularExpression($".*{request.QueryFilter.Search}.*", "i");
                filter = Builders<Domain.MongoEntities.Occupation>.Filter.Or(
                Builders<Domain.MongoEntities.Occupation>.Filter.Regex(x => x.Name, searchRegex),
                Builders<Domain.MongoEntities.Occupation>.Filter.AnyIn(x => x.MajorCodes, request.QueryFilter.Search.Split(','))
            );
            }
            else
            {
                filter = Builders<Domain.MongoEntities.Occupation>.Filter.Empty;
            }

            var results = await _context.Occupations.Find(filter)
                .Skip(request.QueryFilter.PageSize * (request.QueryFilter.PageNumber - 1))
                .Limit(request.QueryFilter.PageSize)
                .ToListAsync(cancellationToken);

            var totalCount = await _context.Occupations.CountDocumentsAsync(filter);

            if (!results.Any())
            {
                return new PagedList<Domain.MongoEntities.Occupation>(new List<Domain.MongoEntities.Occupation>(), 0, 0, 0);
            }

            return new PagedList<Domain.MongoEntities.Occupation>(
                results,
                Convert.ToInt32(totalCount),
                request.QueryFilter.PageNumber,
                request.QueryFilter.PageSize
            );
        }
    }

}
