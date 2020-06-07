using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.WebAPI.Helpers;
using System;
using System.Threading.Tasks;

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
                var path = string.Format("{0}/{1}/{2}.png", id, type, UploadHelper.RandomName(20));
                await UploadHelper.SavePictureAsync(directory, path, file);

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
                await UploadHelper.SavePictureAsync(directory, path, file);

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
                await UploadHelper.SavePictureAsync(directory, path, file);

                return path;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}