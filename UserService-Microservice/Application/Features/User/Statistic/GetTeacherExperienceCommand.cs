using Application.Common.Models.Common;
using Application.Common.Models.UserModel;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Entities;
using Domain.Enumerations;
using Domain.MongoEntities;
using Humanizer;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Net;
using ZstdSharp.Unsafe;

namespace Application.Features.User.Statistic
{
    public class GetTeacherExperienceCommand : IRequest<List<TeacherExperienceResponse>>
    {
    }

    public class GetTeacherExperienceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider) : IRequestHandler<GetTeacherExperienceCommand, List<TeacherExperienceResponse>>
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<List<TeacherExperienceResponse>> Handle(GetTeacherExperienceCommand request, CancellationToken cancellationToken)
        {
            var teacher = await _unitOfWork.TeacherRepository.GetTeacherExperienceCount();
            var responseModels = teacher.Select(s =>
            { 
                return new TeacherExperienceResponse()
                {
                    Range = s.Key,
                    Count = s.Value
                };

            }).Reverse().ToList();
            return responseModels;
        }
    }
}
