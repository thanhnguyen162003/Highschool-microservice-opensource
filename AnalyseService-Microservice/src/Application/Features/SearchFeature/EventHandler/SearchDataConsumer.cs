// using System.Net;
// using Application.Common.Kafka;
// using Application.KafkaMessageModel;
//
// namespace Application.Features.SearchFeature.EventHandler;
//
// public class SearchDataConsumer : KafkaConsumerBase<SearchDataConsumeModel>
// {
//     public SearchDataConsumer(IConfiguration configuration, ILogger<SearchDataConsumer> logger, IServiceProvider serviceProvider)
//         : base(configuration, logger, serviceProvider,
//             new List<string> {"subject_created", "lesson_created","chapter_created","user_registered" },
//             "search_data_consumer_group")
//     {
//     }
//     
//     protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
//     {
//         // var _sender = serviceProvider.GetRequiredService<ISender>();
//         // var _logger = serviceProvider.GetRequiredService<ILogger<SearchDataConsumer>>();
//         //
//         //
//         // SubjectUpdateConsumeModel subjectImageModel = JsonSerializer.Deserialize<SubjectUpdateConsumeModel>(message);
//         // SubjectModel subjectModel = new SubjectModel()
//         // {
//         //     Id = subjectImageModel.SubjectId,
//         //     Image = subjectImageModel.ImageUrl,
//         // };
//         // var command = new UpdateSubjectCommand()
//         // {
//         //     SubjectModel = subjectModel
//         // };
//         // var result = await _sender.Send(command);
//         // if (result.Status != HttpStatusCode.OK)
//         // {
//         //     _logger.Log(LogLevel.Error, "Error consume image message from Kafka");
//         // }
//     }
// }
