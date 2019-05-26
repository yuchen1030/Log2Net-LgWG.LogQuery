using Abp.Dependency;
using Abp.Runtime.Session;
using LgWG.LogQuery.LogMonitor;
using LgWG.LogQuery.LogTrace;
using Castle.Core.Logging;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace LgWG.LogQuery.Web.SignalRHub
{
    //即时通讯（聊天）集线器
    public class MyChatHub : Hub, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        public ILogger Logger { get; set; }

        public MyChatHub()
        {
            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
        }

        private readonly ILog_OperateTraceService _logTraceService;
        private readonly ILog_SystemMonitorService _logMonitorService;
        public MyChatHub(ILog_OperateTraceService logTraceService, ILog_SystemMonitorService logMonitorService)
        {
            _logTraceService = logTraceService;
            _logMonitorService = logMonitorService;
        }


        public void SendMessage(string message)
        {
            Clients.All.getMessage(string.Format("User {0}: {1}", AbpSession.UserId, message));
        }

        public async override Task OnConnected()
        {
            await base.OnConnected();
            Logger.Debug("A client connected to MyChatHub: " + Context.ConnectionId);
        }

        public async override Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);
            Logger.Debug("A client disconnected from MyChatHub: " + Context.ConnectionId);
        }
    }



}