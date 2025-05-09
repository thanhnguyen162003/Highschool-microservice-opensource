using Application.Common.Models.MajorCategoryModel;
using Application.Common.Models.MajorModel;
using Application.Common.Models.UniversityMajor;
using Domain.Common.Models;
using Infrastructure.CustomEntities;
using Infrastructure.Data;
using Infrastructure.QueryFilters;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Features.UniversityMajor.Queries
{
    public record GetUniversityMajors : IRequest<PagedList<UniversityMajorResponseModel>>
    {
        public UniversityMajorQueryFilter QueryFilter { get; set; }
    }

    public class GetUniversityMajorsHandler(CareerMongoDatabaseContext context, IMapper mapper, IOptions<PaginationOptions> paginationOptions) : IRequestHandler<GetUniversityMajors, PagedList<UniversityMajorResponseModel>>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

        public async Task<PagedList<UniversityMajorResponseModel>> Handle(GetUniversityMajors request, CancellationToken cancellationToken)
        {
            request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
            request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;

            FilterDefinition<Domain.MongoEntities.UniversityMajor> filter;

            if (!string.IsNullOrEmpty(request.QueryFilter.Search))
            {
                filter = Builders<Domain.MongoEntities.UniversityMajor>.Filter.Or(
                    Builders<Domain.MongoEntities.UniversityMajor>.Filter.Eq(x => x.UniCode, request.QueryFilter.Search),
                    Builders<Domain.MongoEntities.UniversityMajor>.Filter.Eq(x => x.MajorCode, request.QueryFilter.Search)
                );
            }
            else
            {
                filter = Builders<Domain.MongoEntities.UniversityMajor>.Filter.Empty;
            }

            var universityMajors = await _context.UniversityMajors.Find(filter)
                .Skip(request.QueryFilter.PageSize * (request.QueryFilter.PageNumber - 1))
                .Limit(request.QueryFilter.PageSize)
                .ToListAsync(cancellationToken);

            var totalCount = await _context.UniversityMajors.CountDocumentsAsync(filter);

            if (!universityMajors.Any())
            {
                return new PagedList<UniversityMajorResponseModel>(new List<UniversityMajorResponseModel>(), 0, 0, 0);
            }

            // Lấy danh sách mã MajorCode để truy vấn dữ liệu từ Majors
            var majorCodes = universityMajors.Select(um => um.MajorCode).Distinct().ToList();
            var majors = await _context.Majors.Find(Builders<Domain.MongoEntities.Major>.Filter.In(m => m.MajorCode, majorCodes))
                                              .ToListAsync(cancellationToken);

            var majorCategoryCodes = majors.Select(m => m.MajorCategoryCode).Distinct().ToList();

            var majorCategories = await _context.MajorCategories.Find(Builders<Domain.MongoEntities.MajorCategory>.Filter.In(m => m.MajorCategoryCode, majorCategoryCodes)).ToListAsync(cancellationToken);

            // Ánh xạ dữ liệu UniversityMajor với Major
            var response = universityMajors.Select(um =>
            {
                var major = majors.FirstOrDefault(m => m.MajorCode == um.MajorCode);
                var majorCategory = majorCategories.FirstOrDefault(mc => mc.MajorCategoryCode == major.MajorCategoryCode);
                return new UniversityMajorResponseModel
                {
                    Id = um.Id,
                    UniCode = um.UniCode,
                    MajorCode = um.MajorCode,
                    AdmissionMethods = um.AdmissionMethods,
                    Quota = um.Quota,
                    DegreeLevel = um.DegreeLevel,
                    Major = major != null ? new MajorResponseModel
                    {
                        Id = major.Id,
                        MajorCode = major.MajorCode,
                        Name = major.Name,
                        Description = major.Description,
                        SkillYouLearn = major.SkillYouLearn,
                        MajorCategoryCode = major.MajorCategoryCode,
                        MajorCategory =majorCategory != null ? new MajorCategoryResponseModel
                        {
                            Id = majorCategory.Id,
                            MajorCategoryCode = majorCategory.MajorCategoryCode,
                            Name = majorCategory.Name,
                            MBTITypes = majorCategory.MBTITypes,
                            PrimaryHollandTrait = majorCategory.PrimaryHollandTrait,
                            SecondaryHollandTrait = majorCategory.SecondaryHollandTrait
                        } : null
                    } : null
                };
            }).ToList();

            return new PagedList<UniversityMajorResponseModel>(
                response,
                Convert.ToInt32(totalCount),
                request.QueryFilter.PageNumber,
                request.QueryFilter.PageSize
            );
        }
    }

}
