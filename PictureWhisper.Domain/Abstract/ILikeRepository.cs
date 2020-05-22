using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 点赞数据仓库接口
    /// </summary>
    public interface ILikeRepository
    {
        /// <summary>
        /// 根据用户Id和壁纸Id检查是否点赞
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>已点赞返回true，否则返回false</returns>
        Task<bool> QueryAsync(int userId, int wallpaperId);

        /// <summary>
        /// 添加点赞信息
        /// </summary>
        /// <param name="entity">点赞信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        Task<bool> InsertAsync(T_Like entity);

        /// <summary>
        /// 取消点赞
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>取消成功返回true，否则返回false</returns>
        Task<bool> DeleteAsync(int userId, int wallpaperId);
    }
}
