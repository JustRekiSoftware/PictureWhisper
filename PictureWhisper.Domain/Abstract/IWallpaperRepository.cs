using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 壁纸数据仓库接口
    /// </summary>
    public interface IWallpaperRepository
    {
        /// <summary>
        /// 根据Id获取壁纸
        /// </summary>
        /// <param name="id">壁纸Id</param>
        /// <returns>获取成功返回壁纸信息，失败返回null</returns>
        Task<T_Wallpaper> QueryAsync(int id);

        /// <summary>
        /// 搜索壁纸
        /// </summary>
        /// <param name="queryData">搜索关键字</param>
        /// <param name="filterData">分区条件</param>
        /// <param name="orderData">排序条件</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>成功搜索到结果返回壁纸列表，否则返回null</returns>
        Task<List<T_Wallpaper>> QueryAsync(string queryData, short filterData,
            string orderData, int page, int pageSize);

        /// <summary>
        /// 添加壁纸
        /// </summary>
        /// <param name="entity">壁纸信息</param>
        /// <returns>添加成功返回true，失败则返回false</returns>
        Task<bool> InsertAsync(T_Wallpaper entity);

        /// <summary>
        /// 删除壁纸
        /// </summary>
        /// <param name="id">壁纸Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// 获取某分区壁纸
        /// </summary>
        /// <param name="type">分区Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回壁纸列表</returns>
        Task<List<T_Wallpaper>> GetTypeWallpaperAsync(short type, int page, int pageSize);

        /// <summary>
        /// 获取推荐壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="count">获取数量</param>
        /// <returns>返回壁纸列表</returns>
        Task<List<T_Wallpaper>> GetRecommendWallpaperAsync(int id, int count);

        /// <summary>
        /// 获取未审核的壁纸
        /// </summary>
        /// <param name="count">获取数量</param>
        /// <returns>返回壁纸列表</returns>
        Task<List<T_Wallpaper>> GetUnReviewedWallpaperAsync(int count);

        /// <summary>
        /// 获取用户发布的壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回壁纸列表</returns>
        Task<List<T_Wallpaper>> GetPublishedWallpaperAsync(int id, int page, int pageSize);

        /// <summary>
        /// 获取用户收藏的壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回壁纸列表</returns>
        Task<List<T_Wallpaper>> GetFavoriteWallpaperAsync(int id, int page, int pageSize);

        /// <summary>
        /// 获取用户动态
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>壁纸列表</returns>
        Task<List<T_Wallpaper>> GetSpaceWallpaperAsync(int id, int page, int pageSize);

        /// <summary>
        /// 获取今日壁纸
        /// </summary>
        /// <returns>返回壁纸下载路径</returns>
        Task<string> GetTodayWallpaperAsync();
    }
}
