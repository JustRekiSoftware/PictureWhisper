using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace PictureWhisper.Client.BackgroudTask.Tasks
{
    public sealed class AutoSetWallpaperTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            

            deferral.Complete();
        }
    }
}
