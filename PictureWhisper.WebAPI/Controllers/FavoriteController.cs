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
    [Route("api/favorite")]
    [Authorize]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private IFavoriteRepository favoriteRepo;

        public FavoriteController(IFavoriteRepository repo)
        {
            favoriteRepo = repo;
        }

        [HttpGet("{favoritorId}/{wallpaperId}")]
        public async Task<ActionResult<bool>> CheckFavoriteAsync(int favoritorId, int wallpaperId)
        {
            return await favoriteRepo.QueryAsync(favoritorId, wallpaperId);
        }

        [HttpPost]
        public async Task<IActionResult> PostFavoriteAsync(T_Favorite entity)
        {
            var result = await favoriteRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{favoritorId}/{wallpaperId}")]
        public async Task<IActionResult> DeleteFavoriteAsync(int favoritorId, int wallpaperId)
        {
            var result = await favoriteRepo.DeleteAsync(favoritorId, wallpaperId);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}