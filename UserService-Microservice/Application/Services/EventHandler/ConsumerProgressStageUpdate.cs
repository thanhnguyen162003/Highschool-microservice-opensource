using Application.Common.Kafka;
using Application.Constants;
using Domain.Common.Ultils;
using Domain.Enumerations;
using Infrastructure.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Application.Services.EventHandler
{
    public class ConsumerProgressStageUpdate(
                   IConfiguration configuration,
                              ILogger<ConsumerProgressStageUpdate> logger,
                                         IServiceProvider serviceProvider) : KafkaConsumerBase<ConsumerProgressStageUpdate>(configuration, logger, serviceProvider, TopicKafkaConstaints.ProgressStageUpdated, "user_service_group")
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<ConsumerProgressStageUpdate> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var logger = scopedProvider.GetRequiredService<ILogger<ConsumerProgressStageUpdate>>();
            var unitOfWork = scopedProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                var userIdStr = JsonConvert.DeserializeObject<string>(message);
                if (userIdStr == null || !Guid.TryParse(userIdStr, out var userId))
                {
                    return;
                }

                var user = await unitOfWork.UserRepository.GetDetailUser(userId);

                if (user == null)
                {
                    return;
                }

                if (user.RoleId.ToString()!.Equals(((int)RoleEnum.Student).ToString()))
                {
                    switch (user.ProgressStage!.ConvertToValue<ProgressStage>())
                    {
                        case ProgressStage.SubjectInformation:
                            if ((user.UserSubjects.Count > 0) && (!string.IsNullOrEmpty(user.Student!.TypeExam)))
                            {
                                user.ProgressStage = ProgressStage.PersonalityAssessment.ToString();
                            } else
                            {
                                return;
                            }
                            break;
                        case ProgressStage.PersonalityAssessment:
                            if (!string.IsNullOrEmpty(user.Student!.HollandType) && (user.Student.MbtiType != null))
                            {
                                user.ProgressStage = ProgressStage.Completion.ToString();
                            } else
                            {
                                return;
                            }
                            break;
                        default:
                            return;
                    }

                } else if (user.RoleId.ToString()!.Equals(((int)RoleEnum.Teacher).ToString()))
                {
                    switch (user.ProgressStage!.ConvertToValue<ProgressStage>())
                    {
                        case ProgressStage.SubjectInformation:
                            if ((!string.IsNullOrEmpty(user.Teacher!.SubjectsTaught)) && (!string.IsNullOrEmpty(user.Teacher!.GraduatedUniversity) && (!string.IsNullOrEmpty(user.Teacher!.WorkPlace))))
                            {
                                user.ProgressStage = ProgressStage.VerifyTeacher.ToString();
                            } else
                            {
                                return;
                            }
                            break;
                        case ProgressStage.VerifyTeacher:
                            if (user.Teacher!.Verified)
                            {
                                user.ProgressStage = ProgressStage.Completion.ToString();
                            } else
                            {
                                return;
                            }
                            break;
                        default:
                            return;
                    }
                } else
                {
                    return;
                }

                unitOfWork.UserRepository.Update(user);
                if (await unitOfWork.SaveChangesAsync())
                {
                    logger.LogInformation(DataConstaints.UserServiceLogFirst + " Update progress stage successfully.");
                    return;
                }

            } catch (Exception ex)
            {
                logger.LogError(ex, DataConstaints.UserServiceLogFirst + "Error processing message. Adding to retry list.");
            }
        }

    }
}
