using Microsoft.AspNetCore.SignalR;
using PictureWhisper.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Hubs
{
    /// <summary>
    /// SignalR审核处理中心
    /// </summary>
    public class ReviewHub : Hub
    {
        private static object syncRoot = new object();//同步锁
        public static Dictionary<int, string> ConnectionIdDict { get; set; }
            = new Dictionary<int, string>();//记录已连接用户

        public ReviewHub()
        {
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
                ReviewHelper.RemoveAllByUserId(toRemove.Key);//移除该用户正在进行的所有举报处理或审核
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
            ReviewHelper.RemoveAllByUserId(id);//移除该用户正在进行的所有举报处理或审核
        }
    }
}
