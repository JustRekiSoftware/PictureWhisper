using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 收藏数据仓库接口
    /// </summary>
    public interface IFavoriteRepository
    {
        /// <summary>
        /// 根据收藏者Id和壁纸Id检查是否已收藏
        /// </summary>
        /// <param name="favoritorId">收藏者Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>已收藏返回true，否则返回false</returns>
        Task<bool> QueryAsync(int favoritorId, int wallpaperId);

        /// <summary>
        /// 获取收藏列表
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>获取成功返回壁纸列表，否则返回null</returns>
        Task<List<T_Wallpaper>> QueryAsync(int id, int page, int pageSize);

        /// <summary>
        /// 添加收藏信息
        /// </summary>
        /// <param name="entity">收藏信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        Task<bool> InsertAsync(T_Favorite entity);

        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="favoritorId">收藏者Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>取消成功返回true，否则返回false</returns>
        Task<bool> DeleteAsync(int favoritorId, int wallpaperId);
    }
}
