//using Application;
//using Application.Common.Models.UserModel;
//using Domain.Enumerations;
//using Google.Protobuf.WellKnownTypes;
//using Grpc.Core;
//using Humanizer;
//using Infrastructure.Repositories.Interfaces;
//using Microsoft.IdentityModel.Tokens;

//namespace Domain.Services.UserService;

//public class UserService : UserServiceRpc.UserServiceRpcBase
//{
//	private readonly ILogger<UserService> _logger;
//	private readonly IUnitOfWork _unitOfWork;

//	public UserService(ILogger<UserService> logger, IUnitOfWork unitOfWork)
//	{
//		_logger = logger;
//		_unitOfWork = unitOfWork;
//	}

//	public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
//	{
//		var userData = await _unitOfWork.UserRepository.GetUser(request.Username);
//		if (userData is null)
//		{
//			return new UserResponse
//			{
//				Username = request.Username
//			};
//		}

//		var user = new UserResponse
//		{
//			UserId = userData.Id.ToString(),
//			Username = userData.Username,
//			Email = userData.Email,
//			RoleId = userData.RoleId ?? default,
//			Avatar = userData.ProfilePicture,
//			FullName = userData.Fullname,
//        };

//		_logger.LogInformation($"Fetched user details for username: {request.Username}");

//		return user;
//	}

//    public override async Task<UserCountResponse> GetCountUser(UserCountRequest request, ServerCallContext context)
//    {
        

//        if (request.Amount >= 1 && request.IsCount )
//        {
//            if (request.Type.Equals(UserActivityType.Year.ToString()))
//            {
//                var startDate = 12 * request.Amount;
//                var dates = Enumerable.Range(0, startDate)
//                    .Select(i => DateTime.Now.AddMonths(-i))
//                    .ToList();

//                // Fetch all required session statistics in one call
//                var sessionStats = await _unitOfWork.SessionRepository.GetUserActivityForYear(DateTime.Now.AddMonths(-startDate).Date.AtMidnight(), DateTime.Now);

//                // Convert results to a dictionary for quick lookup
//                var sessionStatsDict = sessionStats.ToDictionary(
//                    s => s.Date,
//                    s => s.Data // Assuming Data is a dictionary containing role-based counts
//                );

//                var responseModels = dates.Select(date =>
//            {
//                var key = new DateTime(date.Year, date.Month, 1);
//                var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

//                return new UserActivity
//                {
//                    Date = key.ToString("yyyy-MM-dd"),
//                    Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
//                    Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
//                    Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
//                };
//            }).ToList();
//                var response = new UserCountResponse();
//                response.Activities.AddRange(responseModels);
//                return response;
//            }



//            else if (request.Type.Equals(UserActivityType.Month.ToString()))
//            {
//                var dates = Enumerable.Range(0, 30 * request.Amount + 1)
//                       .Select(i => (DateTime.Now.AddDays(-i)))
//                       .ToList();

//                var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(DateTime.Now.AddMonths(-1 * request.Amount).Date.AtMidnight(), DateTime.Now);

//                // Convert results to a dictionary for quick lookup
//                var sessionStatsDict = sessionStats.ToDictionary(
//                    s => s.Date,
//                    s => s.Data // Assuming Data is a dictionary containing role-based counts
//                );

//                var responseModels = dates.Select(date =>
//                {
//                    var key = new DateTime(date.Year, date.Month, date.Day);
//                    var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

//                    return new UserActivity
//                    {
//                        Date = key.ToString("yyyy-MM-dd"),
//                        Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
//                        Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
//                        Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
//                    };
//                }).Reverse().ToList();
//                var response = new UserCountResponse();
//                response.Activities.AddRange(responseModels);
//                return response;
//            }
//            else if (request.Type.Equals(UserActivityType.Week.ToString()))
//            {
//                var startDate = DateTime.Now.AddDays(-7 * request.Amount).Date.AtMidnight();
//                var dates = Enumerable.Range(0, (7 * request.Amount) + 1)
//                    .Select(i => (DateTime.Now.AddDays(-i)))
//                    .ToList();

//                var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(startDate, DateTime.Now);

//                // Convert results to a dictionary for quick lookup
//                var sessionStatsDict = sessionStats.ToDictionary(
//                    s => s.Date,
//                    s => s.Data // Assuming Data is a dictionary containing role-based counts
//                );

//                var responseModels = dates.Select(date =>
//                {
//                    var key = new DateTime(date.Year, date.Month, date.Day);
//                    var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

//                    return new UserActivity
//                    {
//                        Date = key.ToString("yyyy-MM-dd"),
//                        Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
//                        Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
//                        Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
//                    };
//                }).Reverse().ToList();
//                var response = new UserCountResponse();
//                response.Activities.AddRange(responseModels);
//                return response;
//            }
//            else if ((request.Type.Equals(UserActivityType.Day.ToString())))
//            {
//                var startDate = DateTime.Now.AddDays(-request.Amount).Date.AtMidnight();
//                var dates = Enumerable.Range(0, request.Amount+1)
//                    .Select(i => (DateTime.Now.AddDays(-i)))
//                    .ToList();

//                var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(startDate, DateTime.Now);

//                // Convert results to a dictionary for quick lookup
//                var sessionStatsDict = sessionStats.ToDictionary(
//                    s => s.Date,
//                    s => s.Data // Assuming Data is a dictionary containing role-based counts
//                );

//                var responseModels = dates.Select(date =>
//                {
//                    var key = new DateTime(date.Year, date.Month, date.Day);
//                    var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

//                    return new UserActivity
//                    {
//                        Date = key.ToString("yyyy-MM-dd"),
//                        Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
//                        Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
//                        Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
//                    };
//                }).Reverse().ToList();
//                var response = new UserCountResponse();
//                response.Activities.AddRange(responseModels);
//                return response;
//            }
//            else
//            {
//                var response = new UserCountResponse();
//                return response;
//            }
//        }
//        else if (request.Amount >= 0 && request.IsCount == false)
//        {
//            if (request.Type.Equals(UserActivityType.Year.ToString()))
//            {
//                var startYear = DateTime.Now.Year - request.Amount;
//                var endYear = request.Amount == 0 ? DateTime.Now.Year : DateTime.Now.Year - 1;

//                // Generate months from January to December for the range of years
//                var dates = Enumerable.Range(startYear, endYear - startYear + 1)
//                    .SelectMany(year => Enumerable.Range(1, 12).Select(month => new DateTime(year, month, 1)))
//                    .ToList();

//                // Fetch all required session statistics in one call
//                var sessionStats = await _unitOfWork.SessionRepository.GetUserActivityForYear(
//                    new DateTime(startYear, 1, 1).Date.AtMidnight(),
//                    new DateTime(endYear, 12, 31).Date.At(23, 59, 59, 999)
//                );

//                // Convert results to a dictionary for quick lookup
//                var sessionStatsDict = sessionStats.ToDictionary(
//                    s => s.Date,
//                    s => s.Data // Assuming Data is a dictionary containing role-based counts
//                );

//                var responseModels = dates.Select(date =>
//                {
//                    var key = new DateTime(date.Year, date.Month, 1);
//                    var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

//                    return new UserActivity
//                    {
//                        Date = key.ToString("yyyy-MM-dd"),
//                        Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
//                        Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
//                        Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
//                    };
//                }).ToList();
//                var response = new UserCountResponse();
//                response.Activities.AddRange(responseModels);
//                return response;
//            }
//            else if (request.Type.Equals(UserActivityType.Month.ToString()))
//            {
//                var startMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-request.Amount);
//                var endMonth = request.Amount == 0 ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)): new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1));

//                // Generate months from startMonth to current month
//                var dates = Enumerable.Range(0, (endMonth - startMonth).Days + 1)
//                    .Select(i => startMonth.AddDays(i))
//                    .ToList();

//                // Fetch all required session statistics in one call
//                var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(
//                    new DateTime(startMonth.Year, startMonth.Month, 1).Date.AtMidnight(),
//                    endMonth.At(23, 59, 59, 999)
//                );

//                // Convert results to a dictionary for quick lookup
//                var sessionStatsDict = sessionStats.ToDictionary(
//                    s => s.Date,
//                    s => s.Data // Assuming Data is a dictionary containing role-based counts
//                );

//                var responseModels = dates.Select(date =>
//                {
//                    var key = new DateTime(date.Year, date.Month, date.Day);
//                    var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

//                    return new UserActivity
//                    {
//                        Date = key.ToString("yyyy-MM-dd"),
//                        Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
//                        Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
//                        Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
//                    };
//                }).ToList();
//                var response = new UserCountResponse();
//                response.Activities.AddRange(responseModels);
//                return response;
//            }
//            else if (request.Type.Equals(UserActivityType.Week.ToString()))
//            {
//                var startWeek = DateTime.Now.AddDays(-7 * request.Amount).Date.AtMidnight();
//                var endWeek = request.Amount == 0 ? DateTime.Now.Date.At(23, 59, 59, 999) : DateTime.Now.AddDays(-1).At(23, 59, 59, 999);

//                // Generate weeks from startWeek to current week
//                var dates = Enumerable.Range(0, (endWeek - startWeek).Days + 1)
//                    .Select(i => startWeek.AddDays(i))
//                    .ToList();

//                // Fetch all required session statistics in one call
//                var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(
//                    startWeek,
//                    endWeek
//                );

//                // Convert results to a dictionary for quick lookup
//                var sessionStatsDict = sessionStats.ToDictionary(
//                    s => s.Date,
//                    s => s.Data // Assuming Data is a dictionary containing role-based counts
//                );

//                var responseModels = dates.Select(date =>
//                {
//                    var key = date;
//                    var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

//                    return new UserActivity
//                    {
//                        Date = key.ToString("yyyy-MM-dd"),
//                        Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
//                        Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
//                        Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
//                    };
//                }).ToList();
//                var response = new UserCountResponse();
//                response.Activities.AddRange(responseModels);
//                return response;
//            }
//            else if ((request.Type.Equals(UserActivityType.Day.ToString())))
//            {
//                var startWeek = DateTime.Now.AddDays(-request.Amount).Date.AtMidnight();
//                var endWeek = request.Amount == 0 ? DateTime.Now.Date.At(23, 59, 59, 999) : DateTime.Now.AddDays(-1).At(23, 59, 59, 999);

//                // Generate weeks from startWeek to current week
//                var dates = Enumerable.Range(0, (endWeek - startWeek).Days + 1)
//                    .Select(i => startWeek.AddDays(i))
//                    .ToList();

//                var sessionStats = await _unitOfWork.SessionRepository.GetUserActivity(
//                    startWeek,
//                    endWeek
//                );

//                // Convert results to a dictionary for quick lookup
//                var sessionStatsDict = sessionStats.ToDictionary(
//                    s => s.Date,
//                    s => s.Data // Assuming Data is a dictionary containing role-based counts
//                );

//                var responseModels = dates.Select(date =>
//                {
//                    var key = new DateTime(date.Year, date.Month, date.Day);
//                    var session = sessionStatsDict.GetValueOrDefault(key, new Dictionary<string, int>());

//                    return new UserActivity
//                    {
//                        Date = key.ToString("yyyy-MM-dd"),
//                        Students = session.GetValueOrDefault(RoleEnum.Student.ToString(), 0),
//                        Teachers = session.GetValueOrDefault(RoleEnum.Teacher.ToString(), 0),
//                        Moderators = session.GetValueOrDefault(RoleEnum.Moderator.ToString(), 0)
//                    };
//                }).ToList();
//                var response = new UserCountResponse();
//                response.Activities.AddRange(responseModels);
//                return response;
//            }
//            return new UserCountResponse();
//        }
//        return new UserCountResponse();
//    }
//    public override async Task<UserMediaResponse> GetUserMedia(UserMediaRequest request, ServerCallContext context)
//    {
//        var userData = await _unitOfWork.UserRepository.GetUserForMedia();
//        if (userData is null)
//        {
//			var response = new UserMediaResponse();
//            response.UserId.Add(request.UserId);
//        }
//		if (!request.UserId.Trim().IsNullOrEmpty())
//		{
//            userData = userData.Where(x => x.Id.ToString() == request.UserId).ToList();
//		}
//		var listIds = userData.Select(x => x.Id.ToString()).AsEnumerable();
//        var listUsernames = userData.Select(x => x.Fullname).AsEnumerable();
//        var listImages = userData.Select(x => x.ProfilePicture).AsEnumerable();
//        UserMediaResponse user = new UserMediaResponse();
//		user.UserId.AddRange(listIds);
//        user.Username.AddRange(listUsernames);
//        user.Image.AddRange(listImages);
//        _logger.LogInformation($"Fetched user details for username: {request.UserId}");

//        return user;
//    }

//    public override async Task<UserLoginCountResponse> GetUserLoginCount(UserLoginCountRequest request, ServerCallContext context)
//    {
//        var userData = await _unitOfWork.SessionRepository.GetUserLoginToday();
//        var response = new UserLoginCountResponse();
//        response.Retention.AddRange(userData.Select(x =>
//        {
//            return new UserRetention
//            {
//                Date = x.Date.ToString(),
//                UserId = x.UserId.ToString(),
//                RoleId = x.Role.ToString()
//            };
//        }).ToList());
//        return response;
//    }
//}