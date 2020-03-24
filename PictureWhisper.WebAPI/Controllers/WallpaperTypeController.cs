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
    [Route("api/wallpaper/type")]
    [Authorize]
    [ApiController]
    public class WallpaperTypeController : ControllerBase
    {
        private IWallpaperTypeRepository wallpaperTypeRepo;

        public WallpaperTypeController(IWallpaperTypeRepository repo)
        {
            wallpaperTypeRepo = repo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<T_WallpaperType>> GetWallpaperTypeAsync(short id)
        {
            var result = await wallpaperTypeRepo.QueryAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet]
        public async Task<ActionResult<List<T_WallpaperType>>> GetWallpaperTypesAsync()
        {
            var result = await wallpaperTypeRepo.QueryAsync();
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> PostWallpaperTypeAsync(T_WallpaperType entity)
        {
            var result = await wallpaperTypeRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchWallpaperTypeAsync(short id,
            [FromBody] JsonPatchDocument<T_WallpaperType> jsonPatch)
        {
            var result = await wallpaperTypeRepo.UpdateAsync(id, jsonPatch);
            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallpaperTypeAsync(short id)
        {
            var result = await wallpaperTypeRepo.DeleteAsync(id);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}