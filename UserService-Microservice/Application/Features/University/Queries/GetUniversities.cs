using Application.Common.Models;
using Application.Common.Models.MajorCategoryModel;
using Application.Common.Models.MajorModel;
using Application.Common.Models.UniversityMajor;
using Application.Common.Models.UniversityModel;
using Application.Common.Ultils;
using Dapr.Client;
using Domain.Common;
using Domain.Common.Interfaces.ClaimInterface;
using Domain.Common.Models;
using Domain.MongoEntities;
using Infrastructure.CustomEntities;
using Infrastructure.Data;
using Infrastructure.QueryFilters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace Application.Features.University.Queries
{
    public record GetUniversities : IRequest<PagedList<UniversityResponseModel>>
    {
        public UniversityQueryFilter QueryFilter { get; set; }
    }

	public class GetUniversitiesHandler(CareerMongoDatabaseContext context, IOptions<PaginationOptions> paginationOptions,
        DaprClient client, IDistributedCache cache, IClaimInterface claimInterface) : IRequestHandler<GetUniversities, PagedList<UniversityResponseModel>>
	{
		private readonly CareerMongoDatabaseContext _context = context;
		private readonly PaginationOptions _paginationOptions = paginationOptions.Value;
		private readonly DaprClient _client = client;
		private readonly IDistributedCache _cache = cache;
		private readonly IClaimInterface _claimInterface = claimInterface;

        public async Task<PagedList<UniversityResponseModel>> Handle(GetUniversities request, CancellationToken cancellationToken)
		{
			request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
			request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;

			var cacheKey = CacheKeyGenerator.GenerateUniversityListKey(request.QueryFilter);
			var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

			if (!string.IsNullOrEmpty(cachedData))
			{
				var cacheModel = JsonConvert.DeserializeObject<PagedListCacheModel<UniversityResponseModel>>(cachedData);
				return new PagedList<UniversityResponseModel>(
					cacheModel.Items,
					cacheModel.TotalCount,
					cacheModel.PageNumber,
					cacheModel.PageSize
				);
			}

			var universityQueryable = _context.Universities.AsQueryable();

			if (!string.IsNullOrWhiteSpace(request.QueryFilter.Search))
			{
				var regexPattern = Regex.Escape(request.QueryFilter.Search);
				universityQueryable = universityQueryable.Where(x =>
					Regex.IsMatch(x.unicode, regexPattern, RegexOptions.IgnoreCase) ||
					Regex.IsMatch(x.name, regexPattern, RegexOptions.IgnoreCase));
			}

			if (request.QueryFilter.City != null)
			{
				universityQueryable = universityQueryable.Where(x => x.City == request.QueryFilter.City);
			}
			var test = request.QueryFilter.UniversityId.HasValue;
            if (request.QueryFilter.UniversityId.HasValue)
            {
                universityQueryable = universityQueryable.Where(x => x.id == request.QueryFilter.UniversityId);
            }
            var hasMajorFilters = !string.IsNullOrWhiteSpace(request.QueryFilter.MajorCode) ||
                                 request.QueryFilter.MinTuition.HasValue ||
                                 request.QueryFilter.MaxTuition.HasValue;
			
            if (hasMajorFilters)
            {
                var universityMajorQueryable = _context.UniversityMajors.AsQueryable();
                if (!string.IsNullOrWhiteSpace(request.QueryFilter.MajorCode))
                {
                    var majorCodesList = request.QueryFilter.MajorCode.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

                    if (majorCodesList.Any())
                    {
                        universityMajorQueryable = universityMajorQueryable.Where(x => majorCodesList.Contains(x.MajorCode));
                    }
                }

				if (request.QueryFilter.MinTuition.HasValue)
				{
					universityMajorQueryable = universityMajorQueryable.Where(x => x.TuitionPerYear >= request.QueryFilter.MinTuition.Value);
				}

				if (request.QueryFilter.MaxTuition.HasValue)
				{
					universityMajorQueryable = universityMajorQueryable.Where(x => x.TuitionPerYear <= request.QueryFilter.MaxTuition.Value);
				}

				var universityMajorFilterResult = universityMajorQueryable.Select(x => x.UniCode).Distinct().ToList();
				universityQueryable = universityQueryable.Where(x => universityMajorFilterResult.Contains(x.unicode));
			}

			var totalCount = await universityQueryable.CountAsync(cancellationToken);
			var uniList = universityQueryable
				.Skip(request.QueryFilter.PageSize * (request.QueryFilter.PageNumber - 1))
				.Take(request.QueryFilter.PageSize)
				.ToList();

			if (!uniList.Any())
			{
				return new PagedList<UniversityResponseModel>(new List<UniversityResponseModel>(), 0, 0, 0);
			}

			var uniCodeList = uniList.Select(x => x.unicode).Distinct().ToList();
			var uniMajorList = await _context.UniversityMajors
				.Find(x => uniCodeList.Contains(x.UniCode))
				.ToListAsync(cancellationToken);

			var majorCodes = uniMajorList.Select(um => um.MajorCode).Distinct().ToList();
			var majors = majorCodes.Any()
				? await _context.Majors
					.Find(Builders<Domain.MongoEntities.Major>.Filter.In(m => m.MajorCode, majorCodes))
					.ToListAsync(cancellationToken)
				: new List<Domain.MongoEntities.Major>();

			var majorCategoryCodes = majors.Select(m => m.MajorCategoryCode).Distinct().ToList();
			var majorCategories = majorCategoryCodes.Any()
				? await _context.MajorCategories
					.Find(Builders<Domain.MongoEntities.MajorCategory>.Filter.In(m => m.MajorCategoryCode, majorCategoryCodes))
					.ToListAsync(cancellationToken)
				: new List<Domain.MongoEntities.MajorCategory>();

			List<ProvinceResponseModel> cityDapr = new();
			Dictionary<int, string> cityMap = new();

			try
			{
				cityDapr = await _client.InvokeMethodAsync<List<ProvinceResponseModel>>(
					HttpMethod.Get,
					"document-sidecar",
					"api/v1/information/provinces?PageSize=63&PageNumber=1");

				cityMap = cityDapr
					.Where(c => c.ProvinceId.HasValue)
					.ToDictionary(c => c.ProvinceId!.Value, c => c.ProvinceName ?? string.Empty);
			}
			catch
			{
				cityMap = new Dictionary<int, string>();
			}

			List<SavedUniversity> savedUniversities = null;

			if (_claimInterface.GetCurrentUserId != Guid.Empty)
			{
				savedUniversities = await _context.SavedUniversities.Find(Builders<Domain.MongoEntities.SavedUniversity>.Filter.Eq(m => m.UserId, _claimInterface.GetCurrentUserId))
					.ToListAsync(cancellationToken);
            }

			var results = uniList.Select(uni => new UniversityResponseModel
			{
				Id = uni.id,
				UniCode = uni.unicode,
				Name = uni.name,
				Description = uni.description,
				LogoUrl = uni.LogoUrl,
				City = cityMap.TryGetValue(uni.City, out var provinceName) ? provinceName : uni.City.ToString(),
                IsSaved = savedUniversities?.Any(uni => savedUniversities.Contains(uni)) ?? false,
                updated_at = uni.updated_at,
				created_at = uni.created_at,
				tags = uni.tags,
				admission_details = uni.admission_details,
				field_details = uni.field_details,
				news_details = uni.news_details,
				CityId = uni.City,
                program_details = uni.program_details,
				UniversityMajors = uniMajorList
					.Where(um => um.UniCode == uni.unicode)
					.Select(um =>
					{
						var major = majors.FirstOrDefault(m => m.MajorCode == um.MajorCode);
						var majorCategory = major != null
							? majorCategories.FirstOrDefault(mc => mc.MajorCategoryCode == major.MajorCategoryCode)
							: null;

						return new UniversityMajorResponseModel
						{
							Id = um.Id,
							UniCode = um.UniCode,
							MajorCode = um.MajorCode,
							AdmissionMethods = um.AdmissionMethods,
							Quota = um.Quota,
							DegreeLevel = um.DegreeLevel,
							TuitionPerYear = um.TuitionPerYear,
							YearOfReference = um.YearOfReference,
							Major = major != null ? new MajorResponseModel
							{
								Id = major.Id,
								MajorCode = major.MajorCode,
								Name = major.Name,
								Description = major.Description,
								SkillYouLearn = major.SkillYouLearn,
								MajorCategoryCode = major.MajorCategoryCode,
								MajorCategory = majorCategory != null ? new MajorCategoryResponseModel
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
					}).ToList()
			}).ToList();

			// ✅ Renamed to avoid name conflict
			var pagedCacheModel = new PagedListCacheModel<UniversityResponseModel>
			{
				Items = results,
				TotalCount = totalCount,
				PageNumber = request.QueryFilter.PageNumber,
				PageSize = request.QueryFilter.PageSize
			};

			var serialized = JsonConvert.SerializeObject(pagedCacheModel);

			await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
			}, cancellationToken);

			return new PagedList<UniversityResponseModel>(
				results,
				totalCount,
				request.QueryFilter.PageNumber,
				request.QueryFilter.PageSize
			);
		}
	}
}
