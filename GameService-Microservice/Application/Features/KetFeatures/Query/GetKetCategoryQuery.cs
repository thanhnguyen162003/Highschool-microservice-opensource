using Application.Common.Helper;
using Application.Services.Authentication;
using AutoMapper;
using Domain.Constants;
using Domain.Enums;
using Domain.Models.Common;
using Domain.Models.KetModels;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.KetFeatures.Query
{
    public class GetKetCategoryQuery : IRequest<IEnumerable<KetResponseModel>>
    {
        public KetCategoryQuery Type { get; set; }
        public int NumberKet { get; set; } = 6;
    }

    public class GetKetCategoryQueryHandler : IRequestHandler<GetKetCategoryQuery, IEnumerable<KetResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        private readonly ICacheRepository _cacheRepository;

        public GetKetCategoryQueryHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, 
            IMapper mapper, ICacheRepository cacheRepository)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _mapper = mapper;
            _cacheRepository = cacheRepository;
        }

        public async Task<IEnumerable<KetResponseModel>> Handle(GetKetCategoryQuery request, CancellationToken cancellationToken)
        {
            var key = Utils.GenerateName<GetKetCategoryQuery>(this, new object[] { request });

            if (request.Type == KetCategoryQuery.Recommended)
            {
                if(_authenticationService.IsAuthenticated())
                {
                    return new PagedList<KetResponseModel>();
                }

                // Get data from cache
                var cacheData = await _cacheRepository.GetAsync<PagedList<KetResponseModel>>(key);

                if (cacheData != null)
                {
                    return cacheData;
                }

                var userId = _authenticationService.GetUserId();
                var user = await _unitOfWork.UserRepository.GetById(userId);

                if(user == null)
                {
                    return new PagedList<KetResponseModel>();
                }

                var ketRecommend = await _unitOfWork.KetRepository.GetKetRecommend(user.Kets, request.NumberKet);

                var result = _mapper.Map<PagedList<KetResponseModel>>(ketRecommend);

                // Set data to cache
                await _cacheRepository.SetAsync(StorageRedis.CacheData, key, result);

                return result;
            } else if(request.Type == KetCategoryQuery.NewUpdate)
            {
                // Get data from cache
                var cacheData = await _cacheRepository.GetAsync<PagedList<KetResponseModel>>(key);

                if (cacheData != null)
                {
                    return cacheData;
                }

                var ketNewUpdate = await _unitOfWork.KetRepository.GetNewKets(request.NumberKet);

                var result = _mapper.Map<PagedList<KetResponseModel>>(ketNewUpdate);

                // Set data to cache
                await _cacheRepository.SetAsync(StorageRedis.CacheData, key, result);

                return result;
            } else if(request.Type == KetCategoryQuery.TopKet)
            {
                // Get data from cache
                var cacheData = await _cacheRepository.GetAsync<PagedList<KetResponseModel>>(key);

                if (cacheData != null)
                {
                    return cacheData;
                }

                var ketTop = await _unitOfWork.KetRepository.GetAll(1, request.NumberKet, KetSortBy.TotalPlay.ToString(), true, k => k.Author!);

                var result = _mapper.Map<PagedList<KetResponseModel>>(ketTop);

                // Set data to cache
                await _cacheRepository.SetAsync(StorageRedis.CacheData, key, result);

                return result;
            }

            
            return new PagedList<KetResponseModel>();
        }
    }

}
