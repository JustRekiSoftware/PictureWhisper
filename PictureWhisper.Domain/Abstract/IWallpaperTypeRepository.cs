using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 壁纸分区数据仓库接口
    /// </summary>
    public interface IWallpaperTypeRepository
    {
        /// <summary>
        /// 根据Id获取壁纸分区信息
        /// </summary>
        /// <param name="id">壁纸分区Id</param>
        /// <returns>返回壁纸分区信息</returns>
        Task<T_WallpaperType> QueryAsync(short id);

        /// <summary>
        /// 获取壁纸分区列表
        /// </summary>
        /// <returns>返回壁纸分区列表</returns>
        Task<List<T_WallpaperType>> QueryAsync();

        /// <summary>
        /// 添加壁纸分区
        /// </summary>
        /// <param name="entity">壁纸分区信息</param>
        /// <returns>添加成功，返回true；失败则返回false</returns>
        Task<bool> InsertAsync(T_WallpaperType entity);

        /// <summary>
        /// 更新壁纸分区
        /// </summary>
        /// <param name="id">壁纸分区Id</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功，则返回true；失败，则返回false</returns>
        Task<bool> UpdateAsync(short id, JsonPatchDocument<T_WallpaperType> entity);

        /// <summary>
        /// 删除壁纸分区
        /// </summary>
        /// <param name="id">壁纸分区Id</param>
        /// <returns>删除成功，则返回ture；失败，则返回false</returns>
        Task<bool> DeleteAsync(short id);
    }
}
