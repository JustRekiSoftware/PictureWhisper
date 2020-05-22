using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Controllers
{
    /// <summary>
    /// 收藏控制器
    /// </summary>
    [Route("api/favorite")]
    [Authorize]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private IFavoriteRepository favoriteRepo;//收藏数据仓库

        public FavoriteController(IFavoriteRepository repo)
        {
            favoriteRepo = repo;
        }

        /// <summary>
        /// 检查是否已收藏
        /// </summary>
        /// <param name="favoritorId">用户Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>已收藏，则返回true；否则返回false</returns>
        [HttpGet("{favoritorId}/{wallpaperId}")]
        public async Task<ActionResult<bool>> CheckFavoriteAsync(int favoritorId, int wallpaperId)
        {
            return await favoriteRepo.QueryAsync(favoritorId, wallpaperId);
        }

        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="entity">收藏信息</param>
        /// <returns>收藏成功，则返回200；失败，则返回404</returns>
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

        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="favoritorId">用户Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>取消成功，则返回200；否则返回404</returns>
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