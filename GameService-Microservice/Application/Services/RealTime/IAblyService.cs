using Domain.Models.Common;

namespace Application.Services.RealTime
{
    public interface IAblyService
    {
        Task<APIResponse> SendMessageToPlayer(string roomId, Guid userId, SocketResponse socketResponse);
        Task<APIResponse> SendMessage(string roomId, SocketResponse socketResponse);
    }
}
