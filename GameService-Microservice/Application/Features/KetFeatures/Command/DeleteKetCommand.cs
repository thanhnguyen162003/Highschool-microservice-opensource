using Application.Messages;
using Domain.Models.Common;
using Infrastructure;
using MediatR;
using System.Net;

namespace Application.Features.KetFeatures.Command
{
    public class DeleteKetCommand : IRequest<APIResponse>
    {
        public Guid KetId { get; set; }
    }

    public class DeleteKetCommandHandler : IRequestHandler<DeleteKetCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteKetCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<APIResponse> Handle(DeleteKetCommand request, CancellationToken cancellationToken)
        {
            var ket = await _unitOfWork.KetRepository.GetById(request.KetId);

            if (ket == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound
                };
            }

            var ketContents = await _unitOfWork.KetContentRepository.GetAll(k => k.KetId.Equals(request.KetId));

            await _unitOfWork.KetContentRepository.Delete(ketContents);

            await _unitOfWork.KetRepository.Delete(ket);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.DeleteSuccessfully
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.ServerError
            };

        }
    }
}
