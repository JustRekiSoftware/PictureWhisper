using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Controllers
{
    /// <summary>
    /// 关注控制器
    /// </summary>
    [Route("api/follow")]
    [Authorize]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private IFollowRepository followRepo;//关注数据仓库

        public FollowController(IFollowRepository repo)
        {
            followRepo = repo;
        }

        /// <summary>
        /// 检查是否已关注
        /// </summary>
        /// <param name="followerId">关注者Id</param>
        /// <param name="followedId">被关注者Id</param>
        /// <returns>已关注，则返回true；否则返回false</returns>
        [HttpGet("{followerId}/{followedId}")]
        public async Task<ActionResult<bool>> CheckFollowAsync(int followerId, int followedId)
        {
            return await followRepo.QueryAsync(followerId, followedId);
        }

        /// <summary>
        /// 获取关注列表
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>获取成功，则返回关注列表；失败，则返回404</returns>
        [HttpGet("{id}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_User>>> GetFollowsAsync(int id, int page, int pageSize)
        {
            var result = await followRepo.QueryAsync(id, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// 关注
        /// </summary>
        /// <param name="entity">关注信息</param>
        /// <returns>关注成功，返回200；否则返回404</returns>
        [HttpPost]
        public async Task<IActionResult> PostFollowAsync(T_Follow entity)
        {
            var result = await followRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="followerId">关注者Id</param>
        /// <param name="followedId">被关注者Id</param>
        /// <returns>取消成功，则返回200；失败，则返回404</returns>
        [HttpDelete("{followerId}/{followedId}")]
        public async Task<IActionResult> DeleteFollowAsync(int followerId, int followedId)
        {
            var result = await followRepo.DeleteAsync(followerId, followedId);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}