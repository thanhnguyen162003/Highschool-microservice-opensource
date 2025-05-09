using Application.Common.Models.AssignmentModel;
using Application.Common.Models.ZoneModel;
using AutoMapper;
using Domain.Entity;
using Domain.Models.Common;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.AssignmentFeatures.Queries
{
    public record GetAssignmentQuery : IRequest<PagedList<AssignmentResponseModel>>
    {
        public string? Search { get; set; }

        [Required]
        public int PageSize { get; set; }
        [Required]
        public int PageNumber { get; set; }

        public Guid ZoneId;
        [Required]
        public bool IsAscending { get; set; }
    }

    public class GetAssignmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAssignmentQuery, PagedList<AssignmentResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedList<AssignmentResponseModel>> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.AssignmentRepository.GetAssignment(request.PageNumber, request.PageSize, request.Search, request.IsAscending);
            if (!result.Any())
            {
                return new PagedList<AssignmentResponseModel>(new List<AssignmentResponseModel>(), 0, 0, 0);
            }
            return _mapper.Map<PagedList<AssignmentResponseModel>>(result);
        }
    }
}