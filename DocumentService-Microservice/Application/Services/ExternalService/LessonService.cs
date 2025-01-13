using Grpc.Core;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.ExternalService;

public class LessonService : LessonServiceCheckRpc.LessonServiceCheckRpcBase
{
    private readonly IUnitOfWork _unitOfWork;

    public LessonService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<LessonCheckResponse> CheckLessonExit(LessonCheckRequest request, ServerCallContext context)
    {
        var result = await _unitOfWork.LessonRepository.LessonIdExistAsync(Guid.Parse(request.LessonId.ToString()));

        var response = new LessonCheckResponse();
        if (result is true)
        {
            response.LessonExits.Add(true);
        }else
        {
            response.LessonExits.Add(false);
        }
        return response;
    }
    public override async Task<LessonResponse> GetLessonId(LessonRequest request, ServerCallContext context)
    {
        var lesson = await _unitOfWork.LessonRepository.GetLessonBySubjectId(request.SubjectId);

        var response = new LessonResponse();
        response.LessonId.AddRange(lesson);
        return response;
    }
}