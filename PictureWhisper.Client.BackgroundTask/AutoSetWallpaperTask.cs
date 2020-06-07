using PictureWhisper.Client.Helper;
using System;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.System.UserProfile;

namespace PictureWhisper.Client.BackgroundTask
{
    /// <summary>
    /// 自动设置壁纸后台任务
    /// </summary>
    public sealed class AutoSetWallpaperTask : IBackgroundTask
    {
        private readonly string imageUrl = HttpClientHelper.baseUrl + "wallpaper/today";

        /// <summary>
        /// 执行设置壁纸
        /// </summary>
        /// <param name="taskInstance">后台任务实例</param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();//包含异步方法时，使用BackgroundTaskDeferral确保使用延迟，否则后台任务可能会提前终止

            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var resp = await client.GetAsync(new Uri(imageUrl));//获取今日壁纸下载地址
                if (resp.IsSuccessStatusCode)
                {
                    var url = HttpClientHelper.baseUrl + "download/picture/origin/"
                        + await resp.Content.ReadAsStringAsync();
                    var buffer = await ImageHelper.GetImageBufferAsync(client, url);//获取图片地址
                    if (buffer != null)
                    {
                        var imageFile = await ApplicationData.Current.LocalFolder
                            .CreateFileAsync("AutoSetTemp.png", CreationCollisionOption.ReplaceExisting);
                        await FileIO.WriteBufferAsync(imageFile, buffer);//保存到本地文件夹
                        if (UserProfilePersonalizationSettings.IsSupported() == true)//检查是否能够设置壁纸
                        {
                            var current = UserProfilePersonalizationSettings.Current;
                            var file = await StorageFile.GetFileFromPathAsync(imageFile.Path);
                            await current.TrySetWallpaperImageAsync(file);//设置壁纸
                        }
                    }
                }
            }

            deferral.Complete();
        }
    }
}
