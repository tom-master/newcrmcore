using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewCrmCore.Infrastructure.CommonTools;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.NotifyCenter
{
    public class NotifyHub : Hub
    {
        public async Task<String> RegisterConnection()
        {
            var accountId = Context.GetHttpContext().Request.Query["accountId"];
            var connectionId = Context.ConnectionId;
            await CacheHelper.GetOrSetCacheAsync(new SignalRConnectionCacheKey(accountId), () => Task.Run(() => connectionId));
            Clients.Client(connectionId);
            return connectionId;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var accountId = Context.GetHttpContext().Request.Query["accountId"];
            await CacheHelper.RemoveKeyWhenModify(new SignalRConnectionCacheKey(accountId));
            await base.OnDisconnectedAsync(exception);
        }
    }

    public class CommonNotify
    {
        private readonly IHubContext<NotifyHub> _notify;

        public CommonNotify(IHubContext<NotifyHub> notify)
        {
            _notify = notify;
        }

        public async Task Send(Int32 accountId)
        {
            var connectionId = await CacheHelper.GetOrSetCacheAsync<String>(new SignalRConnectionCacheKey(accountId.ToString()));
            await _notify.Clients.Client(connectionId).SendAsync("message", "wasd123");
        }
    }
}
