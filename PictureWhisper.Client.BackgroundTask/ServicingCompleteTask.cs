using PictureWhisper.Client.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace PictureWhisper.Client.BackgroundTask
{
    /// <summary>
    /// 更新完成后自动执行的后台任务
    /// </summary>
    public sealed class ServicingCompleteTask : IBackgroundTask
    {
        private string[] taskNameArray = new string[]
        {
            typeof(AutoSetWallpaperTask).Name
        };//所有其它后台任务

        /// <summary>
        /// 执行更新完成后的处理
        /// </summary>
        /// <param name="taskInstance">后台任务实例</param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();//包含异步方法时，使用BackgroundTaskDeferral确保使用延迟，否则后台任务可能会提前终止
            foreach (var taskName in taskNameArray)
            {
                BackgroundTaskHelper.UnRegisterBackgroundTask(taskName);//注销后台任务
            }
            if (SQLiteHelper.GetSettingInfo().STI_AutoSetWallpaper)
            {
                await BackgroundTaskHelper.RegisterBackgroundTaskAsync(
                    typeof(AutoSetWallpaperTask),
                    typeof(AutoSetWallpaperTask).Name,
                    new TimeTrigger(60, false),
                    null,
                    true);//注册自动设置壁纸任务
            }

            deferral.Complete();
        }
    }
}
