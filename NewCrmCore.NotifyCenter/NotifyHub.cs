using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace NewCrmCore.NotifyCenter
{
    public class NotifyHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Clients.Client($@"{Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public async Task Send()
        {
            await Clients.All.SendAsync("ReceiveMessage", "wasd123");
        }
    }
}
