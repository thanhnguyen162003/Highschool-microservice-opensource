using Application.Common.Models.DaprModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DaprService.Lesson
{
    public record DaprCheckLessonExit : IRequest<List<bool>>
    {
        public IEnumerable<string> LessonIds { get; set; }
    }
    public class DaprCheckLessonExitHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprCheckLessonExit, List<bool>>
    {
        public async Task<List<bool>> Handle(DaprCheckLessonExit request, CancellationToken cancellationToken)
        {
            var response = new List<bool>();

            foreach (var lessonId in request.LessonIds)
            {
                if (Guid.TryParse(lessonId, out var parsedGuid))
                {
                    var exists = await unitOfWork.LessonRepository.LessonIdExistAsync(parsedGuid);
                    response.Add(exists);
                }
                else
                {
                    response.Add(false);
                }
            }

            return response;
        }
    }
}
