using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Concrete;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Hubs
{
    /// <summary>
    /// SignalR消息通知中心
    /// </summary>
    public class NotifyHub : Hub
    {
        private static object syncRoot = new object();//同步锁
        public static Dictionary<int, string> ConnectionIdDict { get; set; }
            = new Dictionary<int, string>();//记录已连接用户
        private DB_PictureWhisperContext pwContext;

        public NotifyHub(DB_PictureWhisperContext pwContext)
        {
            this.pwContext = pwContext;
        }

        /// <summary>
        /// 处理异常断线
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (ConnectionIdDict.ContainsValue(Context.ConnectionId))
            {
                var toRemove = ConnectionIdDict.FirstOrDefault(p => p.Value == Context.ConnectionId);
                ConnectionIdDict.Remove(toRemove.Key);
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 上线时记录连接信息
        /// </summary>
        /// <param name="id">用户Id</param>
        public void SignIn(int id)
        {
            lock (syncRoot)//上锁
            {
                if (ConnectionIdDict.ContainsKey(id))//掉线后再次连接
                {
                    ConnectionIdDict[id] = Context.ConnectionId;//更新连接信息
                }
                else
                {
                    ConnectionIdDict.Add(id, Context.ConnectionId);//添加连接信息
                }
            }
        }

        /// <summary>
        /// 下线时删除连接信息
        /// </summary>
        /// <param name="id">用户Id</param>
        public void SignOut(int id)
        {
            ConnectionIdDict.Remove(id);
        }

        /// <summary>
        /// 检查新消息
        /// </summary>
        /// <returns></returns>
        public async Task<List<short>> CheckNewMessageAsync(int id, DateTime lastCheckDate)
        {
            var result = new List<short>();
            var count = await pwContext.Comments
                .Where(p => p.C_ReceiverID == id && p.C_Date > lastCheckDate).CountAsync();
            if (count > 0)
            {
                result.Add((short)NotifyMessageType.评论);
            }
            count = await pwContext.Replies
                .Where(p => p.RPL_ReceiverID == id && p.RPL_Date > lastCheckDate).CountAsync();
            if (count > 0)
            {
                result.Add((short)NotifyMessageType.回复);
            }
            count = await pwContext.Reviews
                .Where(p => (p.RV_MsgToReportedID == id || p.RV_MsgToReporterID == id) && p.RV_Date > lastCheckDate).CountAsync();
            if (count > 0)
            {
                result.Add((short)NotifyMessageType.审核);
            }

            return result;
        }
    }
}
