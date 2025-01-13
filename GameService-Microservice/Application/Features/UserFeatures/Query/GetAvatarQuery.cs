using Application.Common.Helper;
using Domain.Constants;
using Domain.Entity;
using Domain.Enums;
using Domain.Models.Common;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.UserFeatures.Query
{
    public class GetAvatarQuery : IRequest<PagedList<Avatar>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public AvatarSortBy SortyBy { get; set; } = AvatarSortBy.rarity;
        public bool IsAscending { get; set; } = true;
    }


    public class GetAvatarQueryHandler : IRequestHandler<GetAvatarQuery, PagedList<Avatar>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheRepository _cache;

        public GetAvatarQueryHandler(IUnitOfWork unitOfWork, ICacheRepository cacheRepository)
        {
            _unitOfWork = unitOfWork;
            _cache = cacheRepository;
        }

        public async Task<PagedList<Avatar>> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
        {
            var key = Utils.GenerateName<GetAvatarQuery>(this, new object[] { request });

            var cacheData = await _cache.GetAsync<PagedList<Avatar>>(key);

            if (cacheData != null)
            {
                return cacheData;
            }

            PagedList<Avatar> result = null!;

            if (request.Page == -1)
            {
                var avatars = await _unitOfWork.AvatarRepository.GetAll(request.SortyBy, request.IsAscending);

                result = new PagedList<Avatar>(avatars);
            } else
            {
                result = await _unitOfWork.AvatarRepository.GetAll(request.Page, request.PageSize,
                            request.SortyBy.ToString(), request.IsAscending);
            }


            // Set data to cache
            await _cache.SetAsync(StorageRedis.CacheData, key, result);

            return result;
        }
    }

}
