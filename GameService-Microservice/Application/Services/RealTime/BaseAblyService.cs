using Application.Messages;
using Domain.Models.Common;
using Domain.Models.Settings;
using IO.Ably;
using IO.Ably.Realtime;
using Microsoft.Extensions.Options;
using System.Net;

namespace Application.Services.RealTime
{
    public class BaseAblyService
    {
        protected readonly AblyRealtime _ably;

        public BaseAblyService(IOptions<AblySetting> ablyOptions)
        {
            _ably = new AblyRealtime(new ClientOptions(ablyOptions.Value.ApiKey)
            {
                EchoMessages = false
            });
        }

        public IRealtimeChannel GetChannel(string channelName)
        {
            return _ably.Channels.Get(channelName);
        }

        public async Task<APIResponse> PublishAsync<T>(string channelName, string eventName, T data)
        {
            var channel = GetChannel(channelName);

            var result = await channel.PublishAsync(eventName, data);

            if (result.IsSuccess)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageAbly.PublishSucess
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageAbly.PublishFailed,
                Data = result.Error
            };
        }
    }
}
