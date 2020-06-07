using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Helpers
{
    /// <summary>
    /// 上传文件帮助类
    /// </summary>
    public class UploadHelper
    {
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="directory">图片保存目录</param>
        /// <param name="path">图片保存路径</param>
        /// <param name="file">图片文件</param>
        /// <returns></returns>
        public static async Task SavePictureAsync(string directory, string path, IFormFile file)
        {
            var fullDirectory = Path.Combine(Directory.GetCurrentDirectory(),
                    "pictures/small/" + directory);//完整目录路径
            if (!Directory.Exists(fullDirectory))//不存在则新建
            {
                Directory.CreateDirectory(fullDirectory);
                fullDirectory = Path.Combine(Directory.GetCurrentDirectory(),
                    "pictures/origin/" + directory);
                Directory.CreateDirectory(fullDirectory);
            }
            var pathSmall = Path.Combine(Directory.GetCurrentDirectory(),
                "pictures/small/" + path);//缩略图保存路径
            var pathOrigin = Path.Combine(Directory.GetCurrentDirectory(),
                "pictures/origin/" + path);//原图保存路径
            await Task.Run(() =>
            {
                //获取图片和缩略图，并保存
                using (Image image = Image.FromStream(file.OpenReadStream()))
                {
                    if (image.Width > 480)
                    {
                        var scale = image.Width / 480.0;
                        var smallImage = image.GetThumbnailImage(480,
                            (int)(image.Height / scale), null, IntPtr.Zero);
                        smallImage.Save(pathSmall, ImageFormat.Png);
                    }
                    image.Save(pathOrigin, ImageFormat.Png);
                }
            });
        }

        /// <summary>
        /// 随机生成文件名
        /// </summary>
        /// <param name="count">文件名长度，默认为20</param>
        /// <returns>返回文件名</returns>
        public static string RandomName(int count = 20)
        {
            StringBuilder result = new StringBuilder(20);
            var rand = new Random();

            while (count-- > 0)
            {
                if (rand.Next(0, 2) == 0)
                {
                    result.Append(rand.Next(0, 10));
                }
                else
                {
                    result.Append((char)rand.Next(97, 123));
                }
            }

            return result.ToString();
        }
    }
}
