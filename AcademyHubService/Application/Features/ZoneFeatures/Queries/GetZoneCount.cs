using Application.Common.Models.ZoneModel;
using AutoMapper;
using Domain.DaprModels;
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
   
    public class GetZoneCount : IRequest<ZoneResponseDapr>
    {
    }

    public class GetZoneCountHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetZoneCount, ZoneResponseDapr>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ZoneResponseDapr> Handle(GetZoneCount request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.ZoneMembershipRepository.GetZoneMemberCount();
            var created = await _unitOfWork.ZoneRepository.GetZoneCreationCount();
            var response = new ZoneResponseDapr
            {
                TotalZoneMember = member + created
            };
            return response;

        }
    }
}

