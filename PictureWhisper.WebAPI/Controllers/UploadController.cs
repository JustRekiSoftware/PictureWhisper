using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PictureWhisper.WebAPI.Controllers
{
    /// <summary>
    /// 上传控制器
    /// </summary>
    [Route("api/upload")]
    [Authorize]
    [ApiController]
    public class UploadController : ControllerBase
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="type">上传类型</param>
        /// <returns>上传成功，则返回下载路径；上传失败，则返回404</returns>
        [HttpPost("picture/{id}/{type}")]
        public async Task<ActionResult<string>> UploadPictureAsync(int id, string type)
        {
            try
            {
                IFormFileCollection files = Request.Form.Files;
                if (files == null || files.Count == 0 || files.Count > 1)
                {
                    return BadRequest();//上传文件不符合规范，则返回400
                }
                var file = files[0];
                var directory = string.Format("{0}/{1}", id, type);
                var path = string.Format("{0}/{1}/{2}.png", id, type, RandomName(20));
                await SavePictureAsync(directory, path, file);

                return path;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 上传今日壁纸
        /// </summary>
        /// <returns>上传成功，则返回下载路径；上传失败，则返回404</returns>
        [HttpPost("picture/today")]
        public async Task<ActionResult<string>> UploadTodayPictureAsync()
        {
            try
            {
                IFormFileCollection files = Request.Form.Files;
                if (files == null || files.Count == 0 || files.Count > 1)
                {
                    return BadRequest();//上传文件不符合规范，则返回400
                }
                var file = files[0];
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                var directory = "today";
                var path = string.Format("today/{0}.png", today);
                await SavePictureAsync(directory, path, file);

                return path;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 上传默认图片
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>上传成功，则返回下载路径；上传失败，则返回404</returns>
        [HttpPost("picture/default/{name}")]
        public async Task<ActionResult<string>> UploadDefaultPictureAsync(string name)
        {
            try
            {
                IFormFileCollection files = Request.Form.Files;
                if (files == null || files.Count == 0 || files.Count > 1)
                {
                    return BadRequest();//上传文件不符合规范，则返回400
                }
                var file = files[0];
                var directory = "default";
                var path = string.Format("default/{0}.png", name);
                await SavePictureAsync(directory, path, file);

                return path;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="directory">图片保存目录</param>
        /// <param name="path">图片保存路径</param>
        /// <param name="file">图片文件</param>
        /// <returns></returns>
        public async Task SavePictureAsync(string directory, string path, IFormFile file)
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
        public string RandomName(int count = 20)
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