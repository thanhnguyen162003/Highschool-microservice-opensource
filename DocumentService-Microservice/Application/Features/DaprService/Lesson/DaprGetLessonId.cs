using Application.Common.Models.CurriculumModel;
using Application.Common.Models;
using Domain.Entities;
using Grpc.Core;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.DaprService.Lesson
{
    public record DaprGetLessonId : IRequest<List<string>>
    {
        public IEnumerable<string> SubjectIds { get; set; }
    }
    public class DaprGetLessonIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetLessonId, List<string>>
    {
        public async Task<List<string>> Handle(DaprGetLessonId request, CancellationToken cancellationToken)
        {
            var lesson = await unitOfWork.LessonRepository.GetLessonBySubjectId(request.SubjectIds);

            return lesson.ToList();
        }
    }
}
