using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;

namespace PictureWhisper.WebAPI.Controllers
{
    [Route("api/like")]
    [Authorize]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private ILikeRepository likeRepo;

        public LikeController(ILikeRepository repo)
        {
            likeRepo = repo;
        }

        [HttpGet("{userId}/{wallpaperId}")]
        public async Task<ActionResult<bool>> CheckLikeAsync(int userId, int wallpaperId)
        {
            return await likeRepo.QueryAsync(userId, wallpaperId);
        }

        [HttpPost]
        public async Task<IActionResult> PostLikeAsync(T_Like entity)
        {
            var result = await likeRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{userId}/{wallpaperId}")]
        public async Task<IActionResult> DeleteLikeAsync(int userId, int wallpaperId)
        {
            var result = await likeRepo.DeleteAsync(userId, wallpaperId);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}