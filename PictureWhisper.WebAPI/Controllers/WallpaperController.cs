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
    /// <summary>
    /// 壁纸控制器
    /// </summary>
    [Route("api/wallpaper")]
    [Authorize]
    [ApiController]
    public class WallpaperController : ControllerBase
    {
        private IWallpaperRepository wallpaperRepo;//壁纸数据仓库

        public WallpaperController(IWallpaperRepository repo)
        {
            wallpaperRepo = repo;
        }

        /// <summary>
        /// 根据壁纸Id获取壁纸
        /// </summary>
        /// <param name="id">壁纸Id</param>
        /// <returns>若获取到，则返回壁纸信息；没获取到，则返回404</returns>
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

        /// <summary>
        /// 搜索壁纸
        /// </summary>
        /// <param name="queryData">搜索关键字</param>
        /// <param name="filterData">壁纸分区</param>
        /// <param name="orderData">排序条件</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>若获取到，则返回壁纸列表；没获取到，则返回404</returns>
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

        /// <summary>
        /// 根据壁纸分区获取壁纸
        /// </summary>
        /// <param name="type">壁纸分区Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>若获取到，则返回壁纸列表；没获取到，则返回404</returns>
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

        /// <summary>
        /// 获取推荐壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="count">获取数量</param>
        /// <returns>若获取到，则返回壁纸列表；若未获取到，则返回404</returns>
        [HttpGet("recommend/{id}/{count}")]
        public async Task<ActionResult<List<T_Wallpaper>>> GetRecommendWallpapersAsync(int id, int count)
        {
            var result = await wallpaperRepo.GetRecommendWallpaperAsync(id, count);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// 获取未审核的壁纸
        /// </summary>
        /// <param name="count">每获取数量</param>
        /// <returns>若获取到，则返回壁纸列表；没获取到，则返回404</returns>
        [HttpGet("unreviewed/{count}")]
        public async Task<ActionResult<List<T_Wallpaper>>> GetUnReviewedWallpapersAsync(int count)
        {
            var result = await wallpaperRepo.GetUnReviewedWallpaperAsync(count);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// 获取用户发布的壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>若获取到，则返回壁纸列表；没获取到，则返回404</returns>
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

        /// <summary>
        /// 获取用户收藏的壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>若获取到，则返回壁纸列表；没获取到，则返回404</returns>
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

        /// <summary>
        /// 获取动态
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>若获取到，则返回壁纸列表；没获取到，则返回404</returns>
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

        /// <summary>
        /// 获取今日壁纸的下载路径
        /// </summary>
        /// <returns>若获取到，则返回壁纸下载路径；没获取到，则返回404</returns>
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

        /// <summary>
        /// 发布壁纸
        /// </summary>
        /// <param name="entity">壁纸信息</param>
        /// <returns>发布成功则返回200；失败则返回404</returns>
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

        /// <summary>
        /// 删除壁纸
        /// </summary>
        /// <param name="id">壁纸Id</param>
        /// <returns>删除成功则返回200；失败则返回404</returns>
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