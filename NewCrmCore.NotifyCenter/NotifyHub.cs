using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace NewCrmCore.NotifyCenter
{
    public class NotifyHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Clients.Client(Context.GetHttpContext().Request.Query.FirstOrDefault().Value);
            await base.OnConnectedAsync();
        }
    }
}
