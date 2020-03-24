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
    [Route("api/upload")]
    [Authorize]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost("picture/{id}/{type}")]
        public async Task<ActionResult<string>> UploadPictureAsync(int id, string type)
        {
            try
            {
                IFormFileCollection files = Request.Form.Files;
                if (files == null || files.Count == 0 || files.Count > 1)
                {
                    return BadRequest();
                }
                var file = files[0];
                var directory = string.Format("{0}/{1}", id, type);
                var path = string.Format("{0}/{1}/{2}.png", id, type, RandomName(20));
                var fullDirectory = Path.Combine(Directory.GetCurrentDirectory(),
                    "pictures/small/" + directory);
                if (!Directory.Exists(fullDirectory))
                {
                    Directory.CreateDirectory(fullDirectory);
                    fullDirectory = Path.Combine(Directory.GetCurrentDirectory(),
                    "pictures/origin/" + directory);
                    Directory.CreateDirectory(fullDirectory);
                }
                var pathSmall = Path.Combine(Directory.GetCurrentDirectory(),
                    "pictures/small/" + path);
                var pathOrigin = Path.Combine(Directory.GetCurrentDirectory(),
                    "pictures/origin/" + path);
                await Task.Run(() =>
                {
                    Image image = Image.FromStream(file.OpenReadStream());
                    if (image.Width > 200)
                    {
                        var scale = image.Width / 200.0;
                        var smallImage = image.GetThumbnailImage(200,
                            (int)(image.Height / scale), null, IntPtr.Zero);
                        smallImage.Save(pathSmall, ImageFormat.Png);
                    }
                    image.Save(pathOrigin, ImageFormat.Png);
                });

                return path;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        public string RandomName(int count)
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