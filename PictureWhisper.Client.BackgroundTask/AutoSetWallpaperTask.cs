using PictureWhisper.Client.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.System.UserProfile;

namespace PictureWhisper.Client.BackgroundTask
{
    public sealed class AutoSetWallpaperTask : IBackgroundTask
    {
        private readonly string imageUrl = HttpClientHelper.baseUrl + "wallpaper/today";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var resp = await client.GetAsync(new Uri(imageUrl));
                if (resp.IsSuccessStatusCode)
                {
                    var url = HttpClientHelper.baseUrl + "download/picture/origin/"
                        + await resp.Content.ReadAsStringAsync();
                    var buffer = await ImageHelper.GetImageBufferAsync(client, url);
                    if (buffer != null)
                    {
                        var imageFile = await ApplicationData.Current.LocalFolder
                        .CreateFileAsync("AutoSetTemp.png", CreationCollisionOption.ReplaceExisting);
                        await FileIO.WriteBufferAsync(imageFile, buffer);
                        if (UserProfilePersonalizationSettings.IsSupported() == true)
                        {
                            var current = UserProfilePersonalizationSettings.Current;
                            var file = await StorageFile.GetFileFromPathAsync(imageFile.Path);
                            await current.TrySetWallpaperImageAsync(file);
                        }
                    }
                }
            }

            deferral.Complete();
        }
    }
}
