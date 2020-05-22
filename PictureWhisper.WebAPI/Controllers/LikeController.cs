using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Controllers
{
    /// <summary>
    /// 点赞控制器
    /// </summary>
    [Route("api/like")]
    [Authorize]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private ILikeRepository likeRepo;//点赞数据仓库

        public LikeController(ILikeRepository repo)
        {
            likeRepo = repo;
        }

        /// <summary>
        /// 检查是否已点赞
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>已点赞，则返回true；否则返回false</returns>
        [HttpGet("{userId}/{wallpaperId}")]
        public async Task<ActionResult<bool>> CheckLikeAsync(int userId, int wallpaperId)
        {
            return await likeRepo.QueryAsync(userId, wallpaperId);
        }

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="entity">点赞信息</param>
        /// <returns>点赞成功，则返回200；失败，则返回404</returns>
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

        /// <summary>
        /// 取消点赞
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>取消成功，则返回200；失败，则返回404</returns>
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