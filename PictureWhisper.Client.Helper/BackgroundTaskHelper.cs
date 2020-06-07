using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace PictureWhisper.Client.Helper
{
    /// <summary>
    /// 后台任务帮助类
    /// </summary>
    public static class BackgroundTaskHelper
    {
        /// <summary>
        /// 注册进程外后台任务
        /// </summary>
        /// <param name="taskEntryPoint">后台任务类的Type</param>
        /// <param name="taskName">后台任务名</param>
        /// <param name="trigger">触发器</param>
        /// <param name="condition">任务运行条件</param>
        /// <param name="IsNetworkRequested">指示后台任务是否需要网络连接</param>
        /// <returns>返回已注册的后台任务</returns>
        public static async Task<BackgroundTaskRegistration> RegisterBackgroundTaskAsync(Type taskEntryPoint,
                                                                        string taskName,
                                                                        IBackgroundTrigger trigger,
                                                                        IBackgroundCondition condition,
                                                                        bool IsNetworkRequested)
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status == BackgroundAccessStatus.Unspecified
                || status == BackgroundAccessStatus.DeniedByUser
                || status == BackgroundAccessStatus.DeniedBySystemPolicy)//无法注册进程外后台任务
            {
                return null;
            }

            foreach (var cur in BackgroundTaskRegistration.AllTasks)//检查是否已注册
            {
                if (cur.Value.Name == taskName)
                {
                    return (BackgroundTaskRegistration)cur.Value;
                }
            }

            var builder = new BackgroundTaskBuilder
            {
                Name = taskName,
                TaskEntryPoint = taskEntryPoint.FullName
            };//配置后台任务

            builder.SetTrigger(trigger);//配置触发器

            if (condition != null)
            {
                builder.AddCondition(condition);//配置任务运行条件
            }

            builder.IsNetworkRequested = IsNetworkRequested;//指示是否需要网络连接

            BackgroundTaskRegistration task = builder.Register();//注册后台任务

            return task;
        }

        /// <summary>
        /// 注销后台任务
        /// </summary>
        /// <param name="taskName">后台任务名</param>
        public static void UnRegisterBackgroundTask(string taskName)
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == taskName)
                {
                    cur.Value.Unregister(true);

                    return;
                }
            }
        }

        private static void DefualtOnProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {

        }

        private static void DefualtOnCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {

        }
    }
}
