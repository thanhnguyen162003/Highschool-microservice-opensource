using Application.Common.Models.Common;
using Application.Common.Models.UserModel;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Enumerations;
using Domain.MongoEntities;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Net;
using ZstdSharp.Unsafe;

namespace Application.Features.User.Statistic
{
    public class GetUserAmountCommand : IRequest<UserAmountResponseModel>
    {
        public string UserType { get; set; }
    }

    public class GetUserAmountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetUserAmountCommand, UserAmountResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<UserAmountResponseModel> Handle(GetUserAmountCommand request, CancellationToken cancellationToken)
        {
            if (request.UserType.Equals(UserStatistic.All.ToString()))
            {
                var totalUser = await _unitOfWork.UserRepository.GetTotalUserAmount();
                var thisMonthUser = await _unitOfWork.UserRepository.GetUserAmountCreateThisMonth();
                var lastMonthUser = await _unitOfWork.UserRepository.GetUserAmountCreateLastMonth();
                double percent = 0;
                if (lastMonthUser == 0)
                {
                    percent = (thisMonthUser - lastMonthUser) * 100 / 1;
                }
                else
                {
                    percent = (thisMonthUser - lastMonthUser) * 100 / lastMonthUser;
                }
                return new UserAmountResponseModel()
                {
                    TotalUser = totalUser,
                    ThisMonthUser = thisMonthUser,
                    IncreaseUserPercent = percent
                };
            }
            else if (request.UserType.Equals(UserStatistic.Student.ToString()))
            {
                var student = await _unitOfWork.StudentRepository.GetTotalStudentAmount();
                return new UserAmountResponseModel()
                {
                    TotalStudent = student
                };
            }
            else if (request.UserType.Equals(UserStatistic.Teacher.ToString()))
            {
                var teacher = await _unitOfWork.TeacherRepository.GetTotalTeacherAmount();
                return new UserAmountResponseModel()
                {
                    TotalTeacher = teacher
                };
            }
            else if (request.UserType.Equals(UserStatistic.Active.ToString()))
            {
                var activeUser = await _unitOfWork.UserRepository.GetTotalActiveUserAmount();
                return new UserAmountResponseModel()
                {
                    TotalActive = activeUser
                };
            }
            else if (request.UserType.Equals(UserStatistic.Block.ToString()))
            {
                var blockedUser = await _unitOfWork.UserRepository.GetTotalBlockedUserAmount();
                return new UserAmountResponseModel() 
                {
                    TotalBlocked = blockedUser
                };
            }
            else if (request.UserType.Equals(UserStatistic.Delete.ToString()))
            {
                var deletedUser = await _unitOfWork.UserRepository.GetTotalDeletedUserAmount();
                return new UserAmountResponseModel() 
                {
                    TotalDeleted = deletedUser
                };
            }
            else
            {
                return new UserAmountResponseModel();
            }
        }
    }
}
