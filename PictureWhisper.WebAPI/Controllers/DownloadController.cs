using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}