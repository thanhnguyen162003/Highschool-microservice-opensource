//using System.Net;
//using Application.Common.Models;
//using Application.Common.Models.CalendarModel;
//using Application.Services.CalendarService.Interface;
//using Infrastructure.Repositories.Interfaces;

//namespace Application.Features.CalendarFeature.Commands
//{
//	public record CreateEventCommand : IRequest<ResponseModel>
//	{
//		public EventCalendarCreateModel EventModel { get; set; }
//	}

//	public class CreateEventCommandHandler(
//		IUnitOfWork unitOfWork,
//        IMapper mapper,
//        IEventService eventService,
//		ILogger<CreateEventCommandHandler> logger)
//		: IRequestHandler<CreateEventCommand, ResponseModel>
//	{
//		public async Task<ResponseModel> Handle(CreateEventCommand request, CancellationToken cancellationToken)
//		{
//			try
//			{
//				var mappedEvent = mapper.Map<EventCalendarModel>(request.EventModel);
//				var response = await eventService.CreateEventAsync(mappedEvent);
//                return new ResponseModel(HttpStatusCode.OK, "Ok", response);
//            }
//			catch (Exception e)
//			{
//				logger.LogError(e, "Error occurred while generating flashcards using AI.");
//				return new ResponseModel(HttpStatusCode.InternalServerError, "An unexpected error occurred.");
//			}
//		}
//	}
//}
