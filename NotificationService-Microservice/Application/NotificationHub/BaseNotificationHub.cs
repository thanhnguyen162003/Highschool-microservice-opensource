//using Application.Common.Models;
//using Microsoft.AspNetCore.SignalR;

//namespace Application.NotificationHub;

//public class BaseNotificationHub : Hub
//{
//    public async Task SendBaseNotification(NotificationModel notification)
//    {
//        await Clients.All.SendAsync("BaseNotification",notification);
//    }
//    public override async Task OnConnectedAsync()
//    {
//        string connectionId = Context.ConnectionId;
//        await base.OnConnectedAsync();
//    }
//}