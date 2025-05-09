using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Constants;
using SharedProjects.ConsumeModel;
using SharedProjects.ConsumeModel.NovuModel;

namespace Application.Features.SubcriberFeature
{
    public record CreateSubscriberCommand : IRequest<ResponseModel>
    {
    }

    public class CreateSubscriberCommandHandler(INovuService novuService, IClaimInterface claimInterface) : IRequestHandler<CreateSubscriberCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(CreateSubscriberCommand request, CancellationToken cancellationToken)
        {
            var subcriberModel = new NovuSubcriberModel()
            {
                Id = claimInterface.GetCurrentUserId,
                Email = claimInterface.GetCurrentEmail,
                LastName = claimInterface.GetCurrentUsername,
            };

            var isSuccess = await novuService.CreateSubscriber(subcriberModel);

            if (isSuccess)
            {
                return new ResponseModel()
                {
                    Status = System.Net.HttpStatusCode.OK,
                    Message = "Đã thêm người dùng vào hệ thống thông báo"
                };
            }

            return new ResponseModel()
            {
                Status = System.Net.HttpStatusCode.InternalServerError,
                Message = "Đã có lỗi xảy ra. Vui lòng thử lại sau"
            };
        }
    }
}
