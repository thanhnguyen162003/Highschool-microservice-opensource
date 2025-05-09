using Application.Common.Models.ZoneModel;
using AutoMapper;
using Domain.Entity;
using Domain.Models.Common;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.ZoneFeatures.Queries
{
   
    public class GetZoneDetailQuery : IRequest<ZoneDetailResponseModel>
    {
        public Guid ZoneId { get; set; }
    }

    public class GetZoneDetailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetZoneDetailQuery, ZoneDetailResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ZoneDetailResponseModel> Handle(GetZoneDetailQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.ZoneRepository.GetZoneDetail(request.ZoneId);
            var zone = _mapper.Map<ZoneDetailResponseModel>(result);
            return zone;

        }
    }
}

