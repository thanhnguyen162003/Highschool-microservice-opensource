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
    public class GetUserActivityCommand : IRequest<List<UserActivityResponseModel>>
    {
        public int Amount { get; set; }
        public string UserActivityType { get; set; }
        public bool IsCountFrom { get; set; }
    }

    public class GetUserActivityCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider) : IRequestHandler<GetUserActivityCommand, List<UserActivityResponseModel>>
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<List<UserActivityResponseModel>> Handle(GetUserActivityCommand request, CancellationToken cancellationToken)
        {
            //IsCountFrom = true
            if (request.IsCountFrom)
            {
                if (request.UserActivityType.Equals(UserActivityType.Year.ToString()))
                {
                    var startDate = 12 * request.Amount;
                    var dates = Enumerable.Range(0, startDate)
                        .Select(i => DateTime.Now.AddMonths(-i))
                        .ToList();

                    // Fetch all required session statistics in one call
                    var sessionStats = await _unitOfWork.SessionRepository.GetUserActivityForYear(DateTime.Now.AddMonths(-startDate).Date.AtMidnight(), DateTime.Now);

                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = new DateTime(date.Year, date.Month, 1);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityResponseModel
                        {
                            Date = key,
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).Reverse().ToList();
                    return responseModels;
                }
                else if (request.UserActivityType.Equals(UserActivityType.Month.ToString()))
                {
                    var dates = Enumerable.Range(0, 30 * request.Amount + 1)
                       .Select(i => (DateTime.Now.AddDays(-i)))
                       .ToList();

                    var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(DateTime.Now.AddMonths(-1 * request.Amount).Date.AtMidnight(), DateTime.Now);

                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = new DateTime(date.Year, date.Month, date.Day);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityResponseModel
                        {
                            Date = key,
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).Reverse().ToList();
                    return responseModels;
                }
                else if (request.UserActivityType.Equals(UserActivityType.Week.ToString()))
                {
                    var startDate = DateTime.Now.AddDays(-7 * request.Amount).Date.AtMidnight();
                    var dates = Enumerable.Range(0, (7 * request.Amount) + 1)
                        .Select(i => (DateTime.Now.AddDays(-i)))
                        .ToList();

                    var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(startDate, DateTime.Now);

                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = new DateTime(date.Year, date.Month, date.Day);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityResponseModel
                        {
                            Date = key,
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).Reverse().ToList();
                    return responseModels;
                }
                else if ((request.UserActivityType.Equals(UserActivityType.Day.ToString())))
                {
                    var startDate = DateTime.Now.AddDays(-request.Amount).Date.AtMidnight();
                    var dates = Enumerable.Range(0, request.Amount + 1)
                        .Select(i => (DateTime.Now.AddDays(-i)))
                        .ToList();

                    var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(startDate, DateTime.Now);
                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = new DateTime(date.Year, date.Month, date.Day);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityResponseModel
                        {
                            Date = key,
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).Reverse().ToList();
                    return responseModels;
                }
                else
                {
                    return new List<UserActivityResponseModel>();
                }
            }
            //IsCountFrom = false
            else
            {
                if (request.UserActivityType.Equals(UserActivityType.Year.ToString()))
                {
                    var startYear = DateTime.Now.Year - request.Amount;
                    var endYear = request.Amount == 0 ? DateTime.Now.Year : DateTime.Now.Year - 1;

                    // Generate months from January to December for the range of years
                    var dates = Enumerable.Range(startYear, endYear - startYear + 1)
                        .SelectMany(year => Enumerable.Range(1, 12).Select(month => new DateTime(year, month, 1)))
                        .ToList();

                    // Fetch all required session statistics in one call
                    var sessionStats = await _unitOfWork.SessionRepository.GetUserActivityForYear(
                        new DateTime(startYear, 1, 1).Date.AtMidnight(),
                        new DateTime(endYear, 12, 31).Date.At(23, 59, 59, 999)
                    );

                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = new DateTime(date.Year, date.Month, 1);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityResponseModel
                        {
                            Date = key,
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).ToList();

                    return responseModels;
                }
                else if (request.UserActivityType.Equals(UserActivityType.Month.ToString()))
                {
                    var startMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-request.Amount);
                    var endMonth = request.Amount == 0 ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) : new DateTime(DateTime.Now.Year, DateTime.Now.Month-1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month-1));

                    // Generate months from startMonth to current month
                    var dates = Enumerable.Range(0, (endMonth - startMonth).Days + 1)
                        .Select(i => startMonth.AddDays(i))
                        .ToList();

                    // Fetch all required session statistics in one call
                    var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(
                        new DateTime(startMonth.Year, startMonth.Month, 1).Date.AtMidnight(),
                        endMonth.At(23, 59, 59, 999)
                    );

                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = date;
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityResponseModel
                        {
                            Date = key,
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).ToList();

                    return responseModels;
                }
                else if (request.UserActivityType.Equals(UserActivityType.Week.ToString()))
                {
                    var startWeek = DateTime.Now.AddDays(-7 * request.Amount).Date.AtMidnight();
                    var endWeek = request.Amount == 0 ? DateTime.Now.Date.At(23, 59, 59, 999) : DateTime.Now.AddDays(-1).At(23,59,59,999);

                    // Generate weeks from startWeek to current week
                    var dates = Enumerable.Range(0, (endWeek - startWeek).Days + 1)
                        .Select(i => startWeek.AddDays(i))
                        .ToList();

                    // Fetch all required session statistics in one call
                    var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(
                        startWeek,
                        endWeek
                    );

                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = date;
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityResponseModel
                        {
                            Date = key,
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).ToList();

                    return responseModels;
                }
                else if ((request.UserActivityType.Equals(UserActivityType.Day.ToString())))
                {
                    var startWeek = DateTime.Now.AddDays(-request.Amount).Date.AtMidnight();
                    var endWeek = request.Amount == 0 ? DateTime.Now.Date.At(23, 59, 59, 999) : DateTime.Now.AddDays(-1).At(23, 59, 59, 999);

                    // Generate weeks from startWeek to current week
                    var dates = Enumerable.Range(0, (endWeek - startWeek).Days + 1)
                        .Select(i => startWeek.AddDays(i))
                        .ToList();

                    var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(
                        startWeek,
                        endWeek
                    );
                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = new DateTime(date.Year, date.Month, date.Day);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityResponseModel
                        {
                            Date = key,
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).ToList();
                    return responseModels;
                }
                return new List<UserActivityResponseModel>();
            }
        }
    }
}
