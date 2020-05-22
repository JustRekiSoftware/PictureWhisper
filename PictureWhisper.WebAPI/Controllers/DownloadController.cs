using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Controllers
{
    /// <summary>
    /// 下载控制器
    /// </summary>
    [Route("api/download")]
    [Authorize]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        /// <summary>
        /// 下载默认图片
        /// </summary>
        /// <param name="quality">图片质量</param>
        /// <param name="name">图片名</param>
        /// <returns>返回图片字节数组</returns>
        [HttpGet("picture/{quality}/default/{name}")]
        public async Task<FileContentResult> DownloadDefaultPictureAsync(string quality, string name)
        {
            var realPath = Path.Combine(Directory.GetCurrentDirectory(),
                    string.Format("pictures/{0}/default/{1}", quality, name));
            var result = await System.IO.File.ReadAllBytesAsync(realPath);

            return File(result, "image/png");
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="quality">图片质量</param>
        /// <param name="id">用户Id</param>
        /// <param name="type">图片类型</param>
        /// <param name="name">图片名</param>
        /// <returns>返回图片字节数组</returns>
        [HttpGet("picture/{quality}/{id}/{type}/{name}")]
        public async Task<FileContentResult> DownloadPictureAsync(string quality, int id,
            string type, string name)
        {
            var realPath = Path.Combine(Directory.GetCurrentDirectory(),
                    string.Format("pictures/{0}/{1}/{2}/{3}", quality, id, type, name));
            var result = await System.IO.File.ReadAllBytesAsync(realPath);

            return File(result, "image/png");
        }

        /// <summary>
        /// 下载今日图片
        /// </summary>
        /// <param name="quality">图片质量</param>
        /// <param name="name">图片名</param>
        /// <returns>返回图片字节数组</returns>
        [HttpGet("picture/{quality}/today/{name}")]
        public async Task<FileContentResult> DownloadTodayPictureAsync(string quality, string name)
        {
            var realPath = Path.Combine(Directory.GetCurrentDirectory(),
                    string.Format("pictures/{0}/today/{1}", quality, name));
            var result = await System.IO.File.ReadAllBytesAsync(realPath);

            return File(result, "image/png");
        }
    }
}