using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Helper
{
    /// <summary>
    /// 验证码帮助类
    /// </summary>
    public class IdentityCodeHelper
    {
        public static object syncRoot = new object();//同步锁
        public static Dictionary<int, Tuple<string, DateTime>> IdentityCodes { get; set; }
            = new Dictionary<int, Tuple<string, DateTime>>();
        public static bool IsCodeCleanTaskRunning { get; private set; } = false;
        private static bool stopCodeCleanTask = false;
        public static double CodeCleanDuration { get; set; } = 5;//按分钟数计，指示验证码清理间隔
        public static double CodeLifeTime { get; set; } = 15;//按分钟数计，指示验证码清理间隔

        /// <summary>
        /// 记录用户Id和对应的验证码
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="code">验证码</param>
        public static void AddIdentityCode(int userId, string code)
        {
            lock (syncRoot)
            {
                if (IdentityCodes.ContainsKey(userId))
                {
                    IdentityCodes[userId] = new Tuple<string, DateTime>(code, DateTime.Now);
                }
                else
                {
                    IdentityCodes.Add(userId, new Tuple<string, DateTime>(code, DateTime.Now));
                }
            }
        }

        /// <summary>
        /// 清除用户Id对应的验证码
        /// </summary>
        /// <param name="userId">用户Id</param>
        public static void RemoveIdentityCode(int userId)
        {
            lock (syncRoot)
            {
                if (IdentityCodes.ContainsKey(userId))
                {
                    IdentityCodes.Remove(userId);
                }
            }
        }

        /// <summary>
        /// 开启验证码清理任务
        /// </summary>
        public static void StartCodeCleanTask()
        {
            if (!IsCodeCleanTaskRunning)
            {
                lock (syncRoot)
                {
                    if (!IsCodeCleanTaskRunning)
                    {
                        IsCodeCleanTaskRunning = true;
                        stopCodeCleanTask = false;
                        Task.Run(() =>
                        {
                            while (!stopCodeCleanTask)
                            {
                                var removeIds = IdentityCodes.Where(p => (p.Value.Item2 - DateTime.Now).TotalMinutes
                                    > CodeCleanDuration).Select(p => p.Key).ToList();
                                lock (syncRoot)
                                {
                                    foreach (var id in removeIds)
                                    {
                                        IdentityCodes.Remove(id);
                                    }
                                }
                                Thread.Sleep((int)(CodeCleanDuration * 60 * 1000));
                            }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 关闭验证码清理任务
        /// </summary>
        public static void StopCodeCleanTask()
        {
            stopCodeCleanTask = true;
        }
    }
}
