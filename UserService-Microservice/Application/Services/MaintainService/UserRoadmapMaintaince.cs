// using Application.Constants;
// using Application.ProduceMessage;
// using Domain.Common.Interfaces.KafkaInterface;
// using Infrastructure.Data;
//
// namespace Application.Services.MaintainService;
//
// public class UserRoadmapMaintaince : BackgroundService
// {
//     private readonly IServiceProvider _serviceProvider;
//     private readonly ILogger<UserRoadmapMaintaince> _logger;
//
//     public UserRoadmapMaintaince(IServiceProvider serviceProvider, ILogger<UserRoadmapMaintaince> logger)
//     {
//         _serviceProvider = serviceProvider;
//         _logger = logger;
//     }
//
//     protected override async Task ExecuteAsync(CancellationToken cancellationToken)
//     {
//         while (!cancellationToken.IsCancellationRequested)
//         {
//             using (var scope = _serviceProvider.CreateScope())
//             {
//                 var producerService = scope.ServiceProvider.GetRequiredService<IProducerService>();
//                 var dbContext = scope.ServiceProvider.GetRequiredService<UserDatabaseContext>();
//
//                 var thresholdTime = DateTime.Now.AddMinutes(-30);
//                 var usersWithoutRoadmaps = dbContext.BaseUsers
//                     .Where(user => user.CreatedAt <= thresholdTime && user.Roadmap == null && user.RoleId == 4 && user.IsNewUser == false)
//                     .Include(s=>s.Student)
//                     .Include(u=>u.UserSubjects)
//                     .ToList();
//
//                 foreach (var user in usersWithoutRoadmaps)
//                 {
//                     //to fix
//                     //consider just send user and analyze get from mongo
//                     var message = new UserUpdatedMessage
//                     {
//                         UserId = user.Id,
//                         Address = user.Address,
//                         Grade = user.Student.Grade.Value,
//                         Major = user.Student.Major,
//                         SchoolName = user.Student.SchoolName,
//                         TypeExam = user.Student.TypeExam,
//                         Subjects = user.UserSubjects
//                             .Where(us => us.SubjectId.HasValue) 
//                             .Select(us => us.SubjectId.Value.ToString())
//                             .ToList()
//                     };
//
//                     await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.UserRecommnedRoadmapMissed,user.Id.ToString() ,message);
//                     _logger.LogInformation($"Produced message for user {user.Username} without roadmap.");
//                 }
//             }
//             
//             await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
//         }
//     }
// }
