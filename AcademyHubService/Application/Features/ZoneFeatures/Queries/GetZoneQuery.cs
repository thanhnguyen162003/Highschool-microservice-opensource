using Application.Common.Models.ZoneModel;
using Application.Services.Authentication;
using AutoMapper;
using Dapr.Client;
using Domain.DaprModels;
using Domain.Entity;
using Domain.Enums;
using Domain.Models.Common;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.ZoneFeatures.Queries
{
   
    public class GetZoneQuery : IRequest<PagedList<ZoneResponseModel>>
    {
        public string? Search { get; set; }
        [Required]
        public int PageSize { get; set; }
        [Required]
        public int PageNumber { get; set; }
        [Required]
        public bool IsAscending { get; set; }


    }

    public class GetZoneQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, Services.Authentication.IAuthenticationService claimInterface, DaprClient dapr) : IRequestHandler<GetZoneQuery, PagedList<ZoneResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly Services.Authentication.IAuthenticationService _claimInterface = claimInterface;
        private readonly DaprClient _dapr = dapr;

        public async Task<PagedList<ZoneResponseModel>> Handle(GetZoneQuery request, CancellationToken cancellationToken)
        {
            var userRole = _claimInterface.User.Role;
            PagedList <Zone> result;
            var userId = _claimInterface.User.UserId;
            if (userRole == (int)RoleEnum.Admin || userRole == (int)RoleEnum.Moderator)
            {
                result = await _unitOfWork.ZoneRepository.GetAllZone(request.PageNumber, request.PageSize, request.Search, request.IsAscending);
            }
            else if (userRole == (int)RoleEnum.Student) 
            {
                
                result = await _unitOfWork.ZoneRepository.GetAllZoneForStudent(request.PageNumber, request.PageSize, request.Search, request.IsAscending, userId);
            }
            else if (userRole == (int)RoleEnum.Teacher)
            {
                var members = await _unitOfWork.ZoneMembershipRepository.GetZoneMembershipByStudentId(userId);
                var ids = members.Select(x => x.Value);
                result = await _unitOfWork.ZoneRepository.GetAllZoneForTeacher(request.PageNumber, request.PageSize, request.Search, request.IsAscending, userId, ids);
            }
            else
            {
                return new PagedList<ZoneResponseModel>(new List<ZoneResponseModel>(), 0, 0, 0);
            }

            if (!result.Any())
            {
                return new PagedList<ZoneResponseModel>(new List<ZoneResponseModel>(), 0, 0, 0);
            }

            //author
            var daprResponse = await _dapr.InvokeMethodAsync<UserResponseMediaDapr>(
                    HttpMethod.Get,
                    "user-sidecar",
                    $"api/v1/dapr/user-media"
                );

            var authorLookup = daprResponse.UserId
                 .Select((id, index) => new Author
                 {
                     AuthorId = Guid.Parse(id),
                     AuthorName = daprResponse.Username[index],
                     AuthorImage = daprResponse.Avatar[index]
                 })
                 .ToDictionary(a => a.AuthorId, a => a);

            var zone = _mapper.Map<PagedList<ZoneResponseModel>>(result, opt =>
            {
                opt.Items["UserId"] = userId;
            });
            zone.Items.Select(x =>
            {
                x.Author = authorLookup.GetValueOrDefault<Guid, Author>(x.CreatedBy.Value);
                return x;
            }).ToList();
            return zone;
        }
    }
}

