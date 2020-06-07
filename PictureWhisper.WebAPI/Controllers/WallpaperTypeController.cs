using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Controllers
{
    /// <summary>
    /// 壁纸分区控制器
    /// </summary>
    [Route("api/wallpaper/type")]
    [Authorize]
    [ApiController]
    public class WallpaperTypeController : ControllerBase
    {
        private IWallpaperTypeRepository wallpaperTypeRepo;//壁纸分区数据仓库

        public WallpaperTypeController(IWallpaperTypeRepository repo)
        {
            wallpaperTypeRepo = repo;
        }

        /// <summary>
        /// 根据Id获取壁纸分区
        /// </summary>
        /// <param name="id">壁纸分区Id</param>
        /// <returns>若找到，则返回壁纸分区信息；没找到，则返回404</returns>
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

        /// <summary>
        /// 获取壁纸分区
        /// </summary>
        /// <returns>若获取到，则返回壁纸分区列表；没获取到，则返回404</returns>
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

        /// <summary>
        /// 添加壁纸分区
        /// </summary>
        /// <param name="entity">壁纸分区信息</param>
        /// <returns>添加成功返回200；不成功返回404</returns>
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

        /// <summary>
        /// 更新壁纸分区
        /// </summary>
        /// <param name="id">壁纸分区Id</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功，则返回204；失败则返回404</returns>
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

        /// <summary>
        /// 删除壁纸分区
        /// </summary>
        /// <param name="id">壁纸分区Id</param>
        /// <returns>删除成功，返回204；失败返回404</returns>
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