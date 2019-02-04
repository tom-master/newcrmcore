using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Data.SQL.InternalDataStore;
using Newtonsoft.Json;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.NotifyCenter
{
    public class NotifyHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext().Request.Query["userId"];

            if (!String.IsNullOrEmpty(userId))
            {
                //移除上次意外退出时没有来得及删除的ConnectionId
                await CacheHelper.RemoveKeyWhenModify(new SignalRConnectionCacheKey(userId));

                var connectionId = Context.ConnectionId;
                await CacheHelper.GetOrSetCacheAsync(new SignalRConnectionCacheKey(userId), () => Task.Run(() => connectionId));
                Clients.Client(connectionId);
                await base.OnConnectedAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var userId = Context.GetHttpContext().Request.Query["userId"];

            if (!String.IsNullOrEmpty(userId))
            {
                await CacheHelper.RemoveKeyWhenModify(new SignalRConnectionCacheKey(userId));
                await base.OnDisconnectedAsync(exception);
            }
        }
    }

    public class CommonNotify
    {
        private readonly IHubContext<NotifyHub> _notify;

        public CommonNotify(IHubContext<NotifyHub> notify)
        {
            _notify = notify;
        }

        public async Task SendNotify(Int32 userId, Notify notify)
        {
            using (var dataStore = new SqlContext(Appsetting.Database))
            {
                var result = dataStore.Add(notify);
                if (result > 0)
                {
                    var connectionId = await CacheHelper.GetOrSetCacheAsync<String>(new SignalRConnectionCacheKey(userId.ToString()));
                    await _notify.Clients.Client(connectionId).SendAsync("message", JsonConvert.SerializeObject(notify));
                }
            }
        }
    }
}
