using PictureWhisper.Client.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace PictureWhisper.Client.BackgroundTask
{
    public sealed class ServicingCompleteTask : IBackgroundTask
    {
        private string[] taskNameArray = new string[]
        {
            typeof(AutoSetWallpaperTask).Name
        };

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            foreach (var taskName in taskNameArray)
            {
                BackgroundTaskHelper.UnRegisterBackgroundTask(taskName);
            }

            if (SQLiteHelper.GetSettingInfo().STI_AutoSetWallpaper)
            {
                await BackgroundTaskHelper.RegisterBackgroundTaskAsync(
                    typeof(AutoSetWallpaperTask),
                    typeof(AutoSetWallpaperTask).Name,
                    new TimeTrigger(60, false),
                    null,
                    true);
            }

            deferral.Complete();
        }
    }
}
