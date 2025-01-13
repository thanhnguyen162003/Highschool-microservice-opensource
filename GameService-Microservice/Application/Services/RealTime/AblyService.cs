using Domain.Constants;
using Domain.Models.Common;
using Domain.Models.Settings;
using Microsoft.Extensions.Options;

namespace Application.Services.RealTime
{
    public class AblyService : BaseAblyService, IAblyService
    {
        public AblyService(IOptions<AblySetting> ablyOptions) : base(ablyOptions)
        {
        }

        public async Task<APIResponse> SendMessage(string roomId, SocketResponse socketResponse)
        {
            return await PublishAsync($"{AblyConstant.RoomChannel}:{roomId}", AblyConstant.MessageEvent, socketResponse);
        }

        public async Task<APIResponse> SendMessageToPlayer(string roomId, Guid userId, SocketResponse socketResponse)
        {
            return await PublishAsync($"{AblyConstant.RoomChannel}:{roomId}:{userId}", AblyConstant.MessageEvent, socketResponse);
        }
    }
}
