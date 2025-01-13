using AutoMapper;
using Application.Services.Authentication;
using Domain.Entity;
using Domain.Enums;
using Domain.Models.Common;
using Domain.Models.KetModels;
using Infrastructure;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.KetFeatures.Query
{
    public class GetKetQuery : IRequest<PagedList<KetResponseModel>>
    {
        [Required]
        public KetQueryType TypeGet { get; set; }
        [Required]
        public int PageNumber { get; set; }
        [Required]
        public int PageSize { get; set; }
        public string? Search { get; set; }
        [Required]
        public KetSortBy SortBy { get; set; }
        [Required]
        public bool IsAscending { get; set; }
    }

    public class GetKetQueryHandler : IRequestHandler<GetKetQuery, PagedList<KetResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthenticationService _authenticationService;

        public GetKetQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authenticationService = authenticationService;
        }

        public async Task<PagedList<KetResponseModel>> Handle(GetKetQuery request, CancellationToken cancellationToken)
        {
            PagedList<Ket> kets = new PagedList<Ket>();

            if (request.TypeGet == KetQueryType.MyKet)
            {
                var userId = _authenticationService.GetUserId();
                
                if(request.PageNumber == -1)
                {
                    kets = _unitOfWork.KetRepository.GetAll(request.SortBy, request.IsAscending, request.Search, userId);
                } else
                {
                    kets = await _unitOfWork.KetRepository.GetAll(k =>
                            (request.Search == null || k.Name!.Contains(request.Search) &&
                            k.CreatedBy.Equals(userId)),
                            request.PageNumber, request.PageSize,
                            request.SortBy.ToString(), request.IsAscending, k => k.Author!);
                }

            } else if (request.TypeGet == KetQueryType.All)
            {
                if(request.PageNumber == -1)
                {
                    kets = _unitOfWork.KetRepository.GetAll(request.SortBy, request.IsAscending, request.Search, Guid.Empty);
                } else
                {
                    kets = await _unitOfWork.KetRepository.GetAll(k =>
                                   request.Search == null || k.Name!.Contains(request.Search) &&
                                   !k.Status!.Equals(KetStatus.Private.ToString()),
                                                      request.PageNumber, request.PageSize,
                                                                         request.SortBy.ToString(), request.IsAscending, k => k.Author!);
                }
            }


            return _mapper.Map<PagedList<KetResponseModel>>(kets);
        }
    }
}
