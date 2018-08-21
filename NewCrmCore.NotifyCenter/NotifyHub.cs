using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace NewCrmCore.NotifyCenter
{
    public class NotifyHub : Hub
    {
        public String Register()
        {
            var connectionId = Context.ConnectionId;
            Clients.Client(connectionId);
            return connectionId;
        }
    }

    public class CommonNotify
    {
        private readonly IHubContext<NotifyHub> _notify;

        public CommonNotify(IHubContext<NotifyHub> notify)
        {
            _notify = notify;
        }

        public async Task Send(String connectionId)
        {
            await _notify.Clients.Client(connectionId).SendAsync("message", "wasd123");
        }
    }
}
