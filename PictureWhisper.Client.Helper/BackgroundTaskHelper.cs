using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace PictureWhisper.Client.Helper
{
    public static class BackgroundTaskHelper
    {
        public static async Task<BackgroundTaskRegistration> RegisterBackgroundTaskAsync(Type taskEntryPoint,
                                                                        string taskName,
                                                                        IBackgroundTrigger trigger,
                                                                        IBackgroundCondition condition,
                                                                        bool IsNetworkRequested)
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status == BackgroundAccessStatus.Unspecified
                || status == BackgroundAccessStatus.DeniedByUser
                || status == BackgroundAccessStatus.DeniedBySystemPolicy)
            {
                return null;
            }

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
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
            };

            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            builder.IsNetworkRequested = IsNetworkRequested;

            BackgroundTaskRegistration task = builder.Register();

            return task;
        }

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
