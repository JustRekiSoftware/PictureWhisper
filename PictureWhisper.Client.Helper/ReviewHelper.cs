using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Client.Helper
{
    /// <summary>
    /// 审核处理帮助类
    /// </summary>
    public class ReviewHelper
    {
        public static bool connected = false;
        private static int userId;
        private static HubConnection connection;//与SignalR中心的连接
        private static string connectionUrl = "https://localhost:5001/hubs/reviewhub";

        /// <summary>
        /// 配置连接
        /// </summary>
        public static void ConfigConnect()
        {
            userId = SQLiteHelper.GetSigninInfo().SI_UserID;
            if (connection == null)
            {
                connection = new HubConnectionBuilder()
                    .WithUrl(connectionUrl)
                    .WithAutomaticReconnect()//设置自动重连
                    .Build();
                connection.Reconnected += async (error) =>
                {
                    await SignInAsync();//重连后自动登录
                };
            }
        }

        /// <summary>
        /// 配置客户端函数
        /// </summary>
        /// <typeparam name="T">函数返回值类型</typeparam>
        /// <param name="onName">函数名</param>
        /// <param name="onAction">函数处理</param>
        public static void ConnectionOn<T>(string onName, Action<T> onAction)
        {
            connection.On(onName, onAction);
        }

        /// <summary>
        /// 连接SignalR中心
        /// </summary>
        /// <returns></returns>
        public static async Task StartAsync()
        {
            if (connection.State != HubConnectionState.Disconnected)//已连接或正在重连则直接返回
            {
                return;
            }
            try
            {
                await connection.StartAsync();//开始连接
                connected = true;
            }
            catch (Exception)
            {
                connected = false;
            }
        }

        /// <summary>
        /// 向服务端发送注册请求
        /// </summary>
        /// <returns></returns>
        public static async Task SignInAsync()
        {
            try
            {
                await connection.InvokeAsync("SignIn", userId);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 向服务端发送注销请求
        /// </summary>
        /// <returns></returns>
        public static async Task SignOutAsync()
        {
            try
            {
                await connection.InvokeAsync("SignOut", userId);
            }
            catch (Exception)
            {

            }
        }
    }
}
