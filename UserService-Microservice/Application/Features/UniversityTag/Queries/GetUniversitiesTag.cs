using Application.Common.Models.MajorCategoryModel;
using Application.Common.Models.MajorModel;
using Application.Common.Models.UniversityMajor;
using Application.Common.Models.UniversityModel;
using Domain.Common.Models;
using Domain.MongoEntities;
using Infrastructure.CustomEntities;
using Infrastructure.Data;
using Infrastructure.QueryFilters;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Text.RegularExpressions;

namespace Application.Features.UniversityTag.Queries
{
    public record GetUniversitiesTag : IRequest<PagedList<UniversityTagResponseModel>>
    {
        public UniversityTagQueryFilter QueryFilter { get; set; }
    }

    public class GetUniversitiesTagHandler(CareerMongoDatabaseContext context, IMapper mapper, IOptions<PaginationOptions> paginationOptions) : IRequestHandler<GetUniversitiesTag, PagedList<UniversityTagResponseModel>>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

        public async Task<PagedList<UniversityTagResponseModel>> Handle(GetUniversitiesTag request, CancellationToken cancellationToken)
        {
            request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
            request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;

            FilterDefinition<Domain.MongoEntities.UniversityTags> filter;

            if (!string.IsNullOrEmpty(request.QueryFilter.Search))
            {
                var searchRegex = new BsonRegularExpression($".*{request.QueryFilter.Search}.*", "i");
                filter = Builders<Domain.MongoEntities.UniversityTags>.Filter.Or(
                    Builders<Domain.MongoEntities.UniversityTags>.Filter.Regex(x => x.Name, searchRegex)
                );
            }
            else
            {
                filter = Builders<Domain.MongoEntities.UniversityTags>.Filter.Empty;
            }

            var results = await _context.UniversityTags.Find(filter)
                                .Skip(request.QueryFilter.PageSize * (request.QueryFilter.PageNumber - 1))
                                .Limit(request.QueryFilter.PageSize)
                                .ToListAsync(cancellationToken);
            
            var totalCount = await _context.UniversityTags.CountDocumentsAsync(filter);

            if (!results.Any())
            {
                return new PagedList<UniversityTagResponseModel>(new List<UniversityTagResponseModel>(), 0, 0, 0);
            }
            var mappedResults = _mapper.Map<List<UniversityTagResponseModel>>(results);
            return new PagedList<UniversityTagResponseModel>(
                mappedResults,
                Convert.ToInt32(totalCount),
                request.QueryFilter.PageNumber,
                request.QueryFilter.PageSize
            );
        }
    }
}
