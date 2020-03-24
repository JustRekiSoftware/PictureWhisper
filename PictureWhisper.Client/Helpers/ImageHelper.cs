using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace PictureWhisper.Client.Helpers
{
    public class ImageHelper
    {
        public static async Task<BitmapImage> GetImageAsync(string url)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                return await GetImageAsync(client, url);
            }
        }

        public static async Task<BitmapImage> GetImageAsync(HttpClient client, string url)
        {
            var image = new BitmapImage();
            var resp = await client.GetAsync(new Uri(url));
            if (resp.IsSuccessStatusCode)
            {
                var buffer = await resp.Content.ReadAsBufferAsync();
                using (var stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(buffer);
                    stream.Seek(0);
                    await image.SetSourceAsync(stream);
                }
            }

            return image;
        }

        public static async Task<BitmapImage> FromFileAsync(StorageFile file)
        {
            var image = new BitmapImage();
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                await image.SetSourceAsync(stream);
            }

            return image;
        }
    }
}
