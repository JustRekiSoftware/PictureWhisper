using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;

namespace PictureWhisper.WebAPI.Controllers
{
    [Route("api/wallpaper")]
    [Authorize]
    [ApiController]
    public class WallpaperController : ControllerBase
    {
        private IWallpaperRepository wallpaperRepo;

        public WallpaperController(IWallpaperRepository repo)
        {
            wallpaperRepo = repo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<T_Wallpaper>> GetWallpaperAsync(int id)
        {
            var result = await wallpaperRepo.QueryAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("query/{queryData}/{filterData}/{orderData}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Wallpaper>>> GetWallpapersAsync(string queryData,
            short filterData, string orderData, int page, int pageSize)
        {
            var result = await wallpaperRepo.QueryAsync(queryData, filterData, orderData, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("type/{type}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Wallpaper>>> GetTypeWallpapersAsync(short type,
            int page, int pageSize)
        {
            var result = await wallpaperRepo.GetTypeWallpaperAsync(type, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("recommend/{id}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Wallpaper>>> GetRecommendWallpapersAsync(int id,
            int page, int pageSize)
        {
            var result = await wallpaperRepo.GetRecommendWallpaperAsync(id, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("review/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Wallpaper>>> GetReviewWallpapersAsync(int page, int pageSize)
        {
            var result = await wallpaperRepo.GetUnReviewedWallpaperAsync(page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("published/{id}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Wallpaper>>> GetPublishedWallpapersAsync(int id, int page, int pageSize)
        {
            var result = await wallpaperRepo.GetPublishedWallpaperAsync(id, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("favorite/{id}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Wallpaper>>> GetFavoriteWallpapersAsync(int id, int page, int pageSize)
        {
            var result = await wallpaperRepo.GetFavoriteWallpaperAsync(id, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("space/{id}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Wallpaper>>> GetSpaceWallpapersAsync(int id, int page, int pageSize)
        {
            var result = await wallpaperRepo.GetSpaceWallpaperAsync(id, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("today")]
        public async Task<ActionResult<string>> GetTodayWallpapersAsync()
        {
            var result = await wallpaperRepo.GetTodayWallpaperAsync();
            if (result == null || result == string.Empty)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> PostWallpaperAsync(T_Wallpaper entity)
        {
            var result = await wallpaperRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallpaperAsync(int id)
        {
            var result = await wallpaperRepo.DeleteAsync(id);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}