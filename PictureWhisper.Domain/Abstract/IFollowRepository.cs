using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 关注数据仓库接口
    /// </summary>
    public interface IFollowRepository
    {
        /// <summary>
        /// 根据关注者Id和被关注者Id检查是否已关注
        /// </summary>
        /// <param name="followerId">关注者Id</param>
        /// <param name="followedId">被关注者Id</param>
        /// <returns>已关注返回true，否则返回false</returns>
        Task<bool> QueryAsync(int followerId, int followedId);

        /// <summary>
        /// 获取用户关注列表
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>获取成功返回用户列表，否则返回null</returns>
        Task<List<T_User>> QueryAsync(int id, int page, int pageSize);

        /// <summary>
        /// 添加关注信息
        /// </summary>
        /// <param name="entity">关注信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        Task<bool> InsertAsync(T_Follow entity);

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="followerId">关注者Id</param>
        /// <param name="followedId">被关注者Id</param>
        /// <returns>取消成功返回true，否则返回false</returns>
        Task<bool> DeleteAsync(int followerId, int followedId);
    }
}
