using Application.Common.Models.DaprModel.Academic;
using Application.Common.Models.DaprModel.User;
using Domain.Enumerations;
using Humanizer;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.Dapr.Users
{
    public record DaprGetCountUser : IRequest<UserCountResponseDapr>
    {
        public UserCountRequestDapr UserCount { get; set; }
    }
    public class DaprGetCountUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetCountUser, UserCountResponseDapr>
    {
        public async Task<UserCountResponseDapr> Handle(DaprGetCountUser request, CancellationToken cancellationToken)
        {
            if (request.UserCount.Amount >= 1 && request.UserCount.IsCount)
            {
                if (request.UserCount.Type.Equals(UserActivityType.Year.ToString()))
                {
                    var startDate = 12 * request.UserCount.Amount;
                    var dates = Enumerable.Range(0, startDate)
                        .Select(i => DateTime.Now.AddMonths(-i))
                        .ToList();

                    // Fetch all required session statistics in one call
                    var sessionStats = await unitOfWork.SessionRepository.GetUserActivityForYear(DateTime.Now.AddMonths(-startDate).Date.AtMidnight(), DateTime.Now);

                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = new DateTime(date.Year, date.Month, 1);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityDapr
                        {
                            Date = key.ToString("yyyy-MM-dd"),
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).ToList();
                    var response = new UserCountResponseDapr();
                    response.Activities.AddRange(responseModels);
                    return response;
                }



                else if (request.UserCount.Type.Equals(UserActivityType.Month.ToString()))
                {
                    var dates = Enumerable.Range(0, 30 * request.UserCount.Amount + 1)
                           .Select(i => (DateTime.Now.AddDays(-i)))
                           .ToList();

                    var sessionStats = await unitOfWork.SessionRepository.GetUserActivity(DateTime.Now.AddMonths(-1 * request.UserCount.Amount).Date.AtMidnight(), DateTime.Now);

                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = new DateTime(date.Year, date.Month, date.Day);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityDapr
                        {
                            Date = key.ToString("yyyy-MM-dd"),
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).Reverse().ToList();
                    var response = new UserCountResponseDapr();
                    response.Activities.AddRange(responseModels);
                    return response;
                }
                else if (request.UserCount.Type.Equals(UserActivityType.Week.ToString()))
                {
                    var startDate = DateTime.Now.AddDays(-7 * request.UserCount.Amount).Date.AtMidnight();
                    var dates = Enumerable.Range(0, (7 * request.UserCount.Amount) + 1)
                        .Select(i => (DateTime.Now.AddDays(-i)))
                        .ToList();

                    var sessionStats = await unitOfWork.SessionRepository.GetUserActivity(startDate, DateTime.Now);

                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = new DateTime(date.Year, date.Month, date.Day);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityDapr
                        {
                            Date = key.ToString("yyyy-MM-dd"),
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).Reverse().ToList();
                    var response = new UserCountResponseDapr();
                    response.Activities.AddRange(responseModels);
                    return response;
                }
                else if ((request.UserCount.Type.Equals(UserActivityType.Day.ToString())))
                {
                    var startDate = DateTime.Now.AddDays(-request.UserCount.Amount).Date.AtMidnight();
                    var dates = Enumerable.Range(0, request.UserCount.Amount + 1)
                        .Select(i => (DateTime.Now.AddDays(-i)))
                        .ToList();

                    var sessionStats = await unitOfWork.SessionRepository.GetUserActivity(startDate, DateTime.Now);

                    // Convert results to a dictionary for quick lookup
                    var sessionStatsDict = sessionStats.ToDictionary(
                        s => s.Date,
                        s => s.Data // Assuming Data is a dictionary containing role-based counts
                    );

                    var responseModels = dates.Select(date =>
                    {
                        var key = new DateTime(date.Year, date.Month, date.Day);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityDapr
                        {
                            Date = key.ToString("yyyy-MM-dd"),
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).Reverse().ToList();
                    var response = new UserCountResponseDapr();
                    response.Activities.AddRange(responseModels);
                    return response;
                }
                else
                {
                    var response = new UserCountResponseDapr();
                    return response;
                }
            }
            else if (request.UserCount.Amount >= 0 && request.UserCount.IsCount == false)
            {
                if (request.UserCount.Type.Equals(UserActivityType.Year.ToString()))
                {
                    var startYear = DateTime.Now.Year - request.UserCount.Amount;
                    var endYear = request.UserCount.Amount == 0 ? DateTime.Now.Year : DateTime.Now.Year - 1;

                    // Generate months from January to December for the range of years
                    var dates = Enumerable.Range(startYear, endYear - startYear + 1)
                        .SelectMany(year => Enumerable.Range(1, 12).Select(month => new DateTime(year, month, 1)))
                        .ToList();

                    // Fetch all required session statistics in one call
                    var sessionStats = await unitOfWork.SessionRepository.GetUserActivityForYear(
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

                        return new UserActivityDapr
                        {
                            Date = key.ToString("yyyy-MM-dd"),
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).ToList();
                    var response = new UserCountResponseDapr();
                    response.Activities.AddRange(responseModels);
                    return response;
                }
                else if (request.UserCount.Type.Equals(UserActivityType.Month.ToString()))
                {
                    var startMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-request.UserCount.Amount);
                    var endMonth = request.UserCount.Amount == 0 ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) : new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1));

                    // Generate months from startMonth to current month
                    var dates = Enumerable.Range(0, (endMonth - startMonth).Days + 1)
                        .Select(i => startMonth.AddDays(i))
                        .ToList();

                    // Fetch all required session statistics in one call
                    var sessionStats = await unitOfWork.SessionRepository.GetUserActivity(
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
                        var key = new DateTime(date.Year, date.Month, date.Day);
                        var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

                        return new UserActivityDapr
                        {
                            Date = key.ToString("yyyy-MM-dd"),
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).ToList();
                    var response = new UserCountResponseDapr();
                    response.Activities.AddRange(responseModels);
                    return response;
                }
                else if (request.UserCount.Type.Equals(UserActivityType.Week.ToString()))
                {
                    var startWeek = DateTime.Now.AddDays(-7 * request.UserCount.Amount).Date.AtMidnight();
                    var endWeek = request.UserCount.Amount == 0 ? DateTime.Now.Date.At(23, 59, 59, 999) : DateTime.Now.AddDays(-1).At(23, 59, 59, 999);

                    // Generate weeks from startWeek to current week
                    var dates = Enumerable.Range(0, (endWeek - startWeek).Days + 1)
                        .Select(i => startWeek.AddDays(i))
                        .ToList();

                    // Fetch all required session statistics in one call
                    var sessionStats = await unitOfWork.SessionRepository.GetUserActivity(
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

                        return new UserActivityDapr
                        {
                            Date = key.ToString("yyyy-MM-dd"),
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).ToList();
                    var response = new UserCountResponseDapr();
                    response.Activities.AddRange(responseModels);
                    return response;
                }
                else if ((request.UserCount.Type.Equals(UserActivityType.Day.ToString())))
                {
                    var startWeek = DateTime.Now.AddDays(-request.UserCount.Amount).Date.AtMidnight();
                    var endWeek = request.UserCount.Amount == 0 ? DateTime.Now.Date.At(23, 59, 59, 999) : DateTime.Now.AddDays(-1).At(23, 59, 59, 999);

                    // Generate weeks from startWeek to current week
                    var dates = Enumerable.Range(0, (endWeek - startWeek).Days + 1)
                        .Select(i => startWeek.AddDays(i))
                        .ToList();

                    var sessionStats = await unitOfWork.SessionRepository.GetUserActivity(
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

                        return new UserActivityDapr
                        {
                            Date = key.ToString("yyyy-MM-dd"),
                            Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
                            Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
                            Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
                        };
                    }).ToList();
                    var response = new UserCountResponseDapr();
                    response.Activities.AddRange(responseModels);
                    return response;
                }
                return new UserCountResponseDapr();
            }
            return new UserCountResponseDapr();
        }
    }
}
