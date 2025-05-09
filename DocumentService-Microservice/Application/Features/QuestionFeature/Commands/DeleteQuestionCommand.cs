using Application.Common.Models;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.QuestionFeature.Commands
{
    public record DeleteQuestionsCommand : IRequest<ResponseModel>
    {
        public List<Guid> QuestionIds { get; set; } = new List<Guid>();
    }
    public class DeleteQuestionsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteQuestionsCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResponseModel> Handle(DeleteQuestionsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.QuestionRepository.DeleteManyAsync(
                    q => request.QuestionIds.Contains(q.Id),
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync();

                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.OK,
                    Message = ResponseConstaints.QuestionDeleted,
                    Data = request.QuestionIds
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.InternalServerError,
                    Message = ResponseConstaints.QuestionDeleteFailed,
                    Data = ex.Message
                };
            }
        }
    }

}
