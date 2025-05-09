using Application.Common.Models;
using Application.Common.Models.Common;
using Application.Common.Models.MajorCategoryModel;
using Application.Common.Models.MajorModel;
using Application.Common.Models.UniversityMajor;
using Application.Common.Models.UniversityModel;
using Dapr.Client;
using Domain.Common;
using Domain.Common.Interfaces.ClaimInterface;
using Domain.Common.Models;
using Infrastructure.CustomEntities;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Application.Features.University.Queries
{
    public class GetSavedUniversitiesQuery : IRequest<PagedList<UniversityResponseModel>>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }

	public class GetSavedUniversitiesQueryHandler(
        CareerMongoDatabaseContext context,
        IClaimInterface claimInterface,
        IOptions<PaginationOptions> paginationOptions,
        DaprClient client,
        IDistributedCache cache) : IRequestHandler<GetSavedUniversitiesQuery, PagedList<UniversityResponseModel>>
	{
		private readonly CareerMongoDatabaseContext _context = context;
		private readonly PaginationOptions _paginationOptions = paginationOptions.Value;
		private readonly IClaimInterface _claimInterface = claimInterface;
		private readonly DaprClient _client = client;
		private readonly IDistributedCache _cache = cache;

        public async Task<PagedList<UniversityResponseModel>> Handle(GetSavedUniversitiesQuery request, CancellationToken cancellationToken)
		{
			request.PageNumber = request.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.PageNumber;
			request.PageSize = request.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.PageSize;

			var userId = _claimInterface.GetCurrentUserId;
			var cacheKey = $"universities:save:{userId}:page={request.PageNumber}&size={request.PageSize}";

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

			var savedUniversities = await _context.SavedUniversities
				.Find(savedUniversity => savedUniversity.UserId == userId)
				.SortByDescending(history => history.CreatedAt)
				.Skip(request.PageSize * (request.PageNumber - 1))
				.Limit(request.PageSize)
				.ToListAsync(cancellationToken);

			var universityIds = savedUniversities.Select(saved => saved.UniversityId).ToList();

			var uniList = await _context.Universities
				.Find(uni => universityIds.Contains(uni.id))
				.ToListAsync(cancellationToken);

			var uniCodeList = uniList.Select(x => x.unicode).Distinct().ToList();
			var uniMajorList = await _context.UniversityMajors.Find(x => uniCodeList.Contains(x.UniCode)).ToListAsync(cancellationToken);
			var majorCodes = uniMajorList.Select(um => um.MajorCode).Distinct().ToList();
			var majors = await _context.Majors.Find(Builders<Domain.MongoEntities.Major>.Filter.In(m => m.MajorCode, majorCodes)).ToListAsync(cancellationToken);
			var majorCategoryCodes = majors.Select(m => m.MajorCategoryCode).Distinct().ToList();
			var majorCategories = await _context.MajorCategories.Find(Builders<Domain.MongoEntities.MajorCategory>.Filter.In(m => m.MajorCategoryCode, majorCategoryCodes)).ToListAsync(cancellationToken);

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

			var results = uniList.Select(uni => new UniversityResponseModel()
			{
				Id = uni.id,
				UniCode = uni.unicode,
				Name = uni.name,
				Description = uni.description,
				LogoUrl = uni.LogoUrl,
				City = cityMap.TryGetValue(uni.City, out var provinceName) ? provinceName : uni.City.ToString(),
				IsSaved = true,
				updated_at = uni.updated_at,
				created_at = uni.created_at,
				tags = uni.tags,
				CityId = uni.City,
                admission_details = uni.admission_details,
				field_details = uni.field_details,
				news_details = uni.news_details,
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

			if (!results.Any())
			{
				return new PagedList<UniversityResponseModel>(new List<UniversityResponseModel>(), 0, 0, 0);
			}

			var totalCount = await _context.SavedUniversities
				.CountDocumentsAsync(savedUniversity => savedUniversity.UserId == userId, cancellationToken: cancellationToken);

			var pagedCacheModel = new PagedListCacheModel<UniversityResponseModel>
			{
				Items = results,
				TotalCount = (int)totalCount,
				PageNumber = request.PageNumber,
				PageSize = request.PageSize
			};

			var serialized = JsonConvert.SerializeObject(pagedCacheModel);
			await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
			}, cancellationToken);

			return new PagedList<UniversityResponseModel>(
				results,
				(int)totalCount,
				request.PageNumber,
				request.PageSize
			);
		}
	}
}
