using System.Net;
using Application.Common.Models.CommonModels;
using Application.Common.Models.QuizModel;
using Application.Services.AIService;

namespace Application.Features.DocumentFeature.Commands;

public class GenerateDocumentQuizCommand : IRequest<ResponseModel>
{
    public string DocumentUrl { get; set; } = string.Empty;
    public int NumberOfQuestions { get; set; }
}

public class GenerateDocumentQuizCommandHandler(IAIService aIService) : IRequestHandler<GenerateDocumentQuizCommand, ResponseModel>
{
    private readonly IAIService _aIService = aIService;

    public async Task<ResponseModel> Handle(GenerateDocumentQuizCommand request, CancellationToken cancellationToken)
    {
        var numberOfQuestions = (int)Math.Ceiling((double)request.NumberOfQuestions / 5);

        using var semaphore = new SemaphoreSlim(3);

        // Tạo danh sách các tác vụ
        var tasks = Enumerable.Range(1, numberOfQuestions).Select(async _ =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                return await _aIService.GenerateQuiz(request.DocumentUrl);
            }
            finally
            {
                semaphore.Release();
            }
        });

        var results = await Task.WhenAll(tasks);
        var questions = results.SelectMany(q => q);

        if (questions.Any())
        {
            return new ResponseModel()
            {
                Status = HttpStatusCode.OK,
                Message = "Tạo Quiz Thành công",
                Data = questions
            };
        }

        return new ResponseModel()
        {
            Status = HttpStatusCode.BadRequest,
            Message = "Tạo Quiz Thất bại",
            Data = null
        };
    }

}
