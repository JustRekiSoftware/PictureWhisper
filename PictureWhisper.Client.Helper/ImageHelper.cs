using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace PictureWhisper.Client.Helper
{
    /// <summary>
    /// 图片帮助类
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="url">图片下载地址</param>
        /// <returns>返回BitmapImage</returns>
        public static async Task<BitmapImage> GetImageAsync(string url)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                return await GetImageAsync(client, url);
            }
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="client">Http客户端</param>
        /// <param name="url">图片下载地址</param>
        /// <returns>返回BitmapImage</returns>
        public static async Task<BitmapImage> GetImageAsync(HttpClient client, string url)
        {
            var image = new BitmapImage();
            var resp = await client.GetAsync(new Uri(url));
            if (resp.IsSuccessStatusCode)//下载成功
            {
                var buffer = await resp.Content.ReadAsBufferAsync();//读取Buffer
                using (var stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(buffer);//写入流
                    stream.Seek(0);
                    await image.SetSourceAsync(stream);//为BitmapImage设置流
                }
            }

            return image;
        }

        /// <summary>
        /// 获取图片Buffer
        /// </summary>
        /// <param name="client">Http客户端</param>
        /// <param name="url">图片下载地址</param>
        /// <returns>获取成功返回IBuffer，否则返回null</returns>
        public static async Task<IBuffer> GetImageBufferAsync(HttpClient client, string url)
        {
            var resp = await client.GetAsync(new Uri(url));
            if (resp.IsSuccessStatusCode)
            {
                var buffer = await resp.Content.ReadAsBufferAsync();

                return buffer;
            }

            return null;
        }

        /// <summary>
        /// 从文件读入
        /// </summary>
        /// <param name="file">图片文件的StorageFile对象</param>
        /// <returns>返回BitmapImage</returns>
        public static async Task<BitmapImage> FromFileAsync(StorageFile file)
        {
            var image = new BitmapImage();
            using (var stream = await file.OpenAsync(FileAccessMode.Read))//读取流
            {
                await image.SetSourceAsync(stream);//设置流
            }

            return image;
        }
    }
}
