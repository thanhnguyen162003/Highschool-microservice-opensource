using Application.Common.Models.UserModel;
using Domain.Enumerations;
using Infrastructure.CustomEntities;
using Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;

namespace Application.Features.User.Statistic
{
    public class GetUserGrowthCommand : IRequest<List<UserActivityResponseModel>>
    {
        public int Amount { get; set; }
        public string UserActivityType { get; set; }
        public bool IsCountFrom { get; set; }
    }

    public class GetUserGrowthCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider) : IRequestHandler<GetUserGrowthCommand, List<UserActivityResponseModel>>
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<List<UserActivityResponseModel>> Handle(GetUserGrowthCommand request, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.Now;

            List<SessionStatisticModel> list = new List<SessionStatisticModel>();
            List<DateTime> dates = new List<DateTime>();
            Dictionary<DateTime, Dictionary<string,int>> sessionStatsDict = new Dictionary<DateTime, Dictionary<string, int>>();
            List<UserActivityResponseModel> result = new List<UserActivityResponseModel>();


            if (request.UserActivityType.Equals(UserActivityType.Year.ToString()))
            {
                if (request.IsCountFrom == true)
                {
                    var startDate = 12 * request.Amount;
                    dates = Enumerable.Range(0, startDate)
                        .Select(i => DateTime.Now.AddMonths(-i)).Reverse()
                        .ToList();
                    var endYear = request.IsCountFrom == true ? DateTime.Now : DateTime.Now.AddYears(-1);
                    list = await _unitOfWork.SessionRepository.GetUserGrowthForYear(new DateTime(DateTime.Now.AddMonths(-startDate).Year, DateTime.Now.AddMonths(-startDate).Month, 1, 0, 0, 0), endYear);
                }
                else
                {
                    var startDate = DateTime.Now.Year - request.Amount;
                    var end = request.Amount == 0 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                    dates = Enumerable.Range(DateTime.Now.Year - request.Amount, end - startDate + 1)
                        .SelectMany(year => Enumerable.Range(1, 12).Select(month => new DateTime(year, month, 1)))
                        .ToList();
                    list = await _unitOfWork.SessionRepository.GetUserGrowthForYear(new DateTime(startDate, 1, 1, 0, 0, 0), new DateTime(end, 12, 31, 0, 0, 0));
                }
                sessionStatsDict = list.ToDictionary(
                    s => s.Date,
                    s => s.Data 
                );

                result.AddRange(dates.Select(date =>
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
                }).ToList());

            }
            else if (request.UserActivityType.Equals(UserActivityType.Month.ToString()))
            {
                if (request.IsCountFrom == true)
                {
                    dates = Enumerable.Range(0, 30 * request.Amount + 1)
                    .Select(i => (DateTime.Now.AddDays(-i))).Reverse()
                    .ToList();
                    list = await _unitOfWork.SessionRepository.GetUserGrowth(new DateTime(DateTime.Now.AddMonths(-1 * request.Amount).Year, DateTime.Now.AddMonths(-1 * request.Amount).Month, 1, 0, 0, 0), DateTime.Now);
                }
                else
                {
                    var startMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-request.Amount);
                    var endMonth = request.Amount == 0 ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) : new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1));

        
                    dates = Enumerable.Range(0, (endMonth - startMonth).Days + 1)
                        .Select(i => startMonth.AddDays(i))
                        .ToList();
                    list = await _unitOfWork.SessionRepository.GetUserGrowth(new DateTime(startMonth.Year, startMonth.Month, 1, 0, 0, 0), new DateTime(endMonth.Year, endMonth.Month, endMonth.Day, 23, 59, 59));
                }
                sessionStatsDict = list.ToDictionary(
                        s => s.Date,
                        s => s.Data 
                    );

                result = dates.Select(date =>
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

            }
            else if (request.UserActivityType.Equals(UserActivityType.Week.ToString()))
            {
                var now = DateTime.Now;
                int daysToMonday = ((int)now.DayOfWeek + 6) % 7;
                var currentWeekMonday = now.AddDays(-daysToMonday).Date;
                var currentWeekSunday = currentWeekMonday.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59); 



                if (request.Amount == 0)
                {
                    var startDate = currentWeekMonday;
                    var endDate = currentWeekSunday; 
                    dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(i => startDate.AddDays(i))
                        .ToList();
                    list = await _unitOfWork.SessionRepository.GetUserGrowth(startDate, endDate);
                }
                else if (request.IsCountFrom)
                {
                    var startDate = currentWeekMonday.AddDays(-7 * request.Amount);
                    var endDate = currentWeekSunday;
                    dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(i => startDate.AddDays(i))
                        .ToList();
                    list = await _unitOfWork.SessionRepository.GetUserGrowth(startDate, endDate);
                }
                else
                {

                    var startDate = currentWeekMonday.AddDays(-7 * request.Amount);
                    var endDate = currentWeekMonday.AddDays(-7 * (request.Amount - 1)).AddDays(-1).Date.AddHours(23).AddMinutes(59).AddSeconds(59); 
                    dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                        .Select(i => startDate.AddDays(i))
                        .ToList();
                    list = await _unitOfWork.SessionRepository.GetUserGrowth(startDate, endDate);
                }

                sessionStatsDict = list.ToDictionary(
                        s => s.Date,
                        s => s.Data
                    );

                result = dates.Select(date =>
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
            }
            else if (request.UserActivityType.Equals(UserActivityType.Day.ToString()))
            {
                if (request.IsCountFrom == true)
                {
                    var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-request.Amount).Day, 0, 0, 0);
                    dates = Enumerable.Range(0, request.Amount + 1)
                        .Select(i => (startDate.AddDays(i)))
                        .ToList();
                    list = await _unitOfWork.SessionRepository.GetUserGrowth(new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0), DateTime.Now);
                }
                else
                {
                    var startWeek = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-request.Amount).Day, 0, 0, 0);
                    var endWeek = request.Amount == 0 ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-1).Day, 23, 59, 59);

                    // Generate weeks from startWeek to current week
                    dates = Enumerable.Range(0, (endWeek - startWeek).Days + 1)
                        .Select(i => startWeek.AddDays(i))
                        .ToList();
                    list = await _unitOfWork.SessionRepository.GetUserGrowth(startWeek, endWeek);
                }
                sessionStatsDict = list.ToDictionary(
                        s => s.Date,
                        s => s.Data 
                    );

                result = dates.Select(date =>
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
            }

            else { 
                return new List<UserActivityResponseModel>();
            }
            return result;
        }
    }
}
