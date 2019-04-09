using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Data.SQL.Mapper;
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
            using (var mapper = new EntityMapper())
            {
                var result = mapper.Add(notify);
                if (result.Id != 0)
                {
                    var connectionId = await CacheHelper.GetOrSetCacheAsync<String>(new SignalRConnectionCacheKey(userId.ToString()));
                    await _notify.Clients.Client(connectionId).SendAsync("message", JsonConvert.SerializeObject(notify));
                }
            }
        }
    }
}
