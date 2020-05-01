using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Controllers
{
    [Route("api/download")]
    [Authorize]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        [HttpGet("picture/{quality}/default/{name}")]
        public async Task<FileContentResult> DownloadDefaultPictureAsync(string quality, string name)
        {
            var realPath = Path.Combine(Directory.GetCurrentDirectory(),
                    string.Format("pictures/{0}/default/{1}", quality, name));
            var result = await System.IO.File.ReadAllBytesAsync(realPath);

            return File(result, "image/png");
        }

        [HttpGet("picture/{quality}/{id}/{type}/{name}")]
        public async Task<FileContentResult> DownloadPictureAsync(string quality, int id,
            string type, string name)
        {
            var realPath = Path.Combine(Directory.GetCurrentDirectory(),
                    string.Format("pictures/{0}/{1}/{2}/{3}", quality, id, type, name));
            var result = await System.IO.File.ReadAllBytesAsync(realPath);

            return File(result, "image/png");
        }

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