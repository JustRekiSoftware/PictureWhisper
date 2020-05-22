using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 评论数据仓库接口
    /// </summary>
    public interface ICommentRepository
    {
        /// <summary>
        /// 根据Id获取评论
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns>返回评论</returns>
        Task<T_Comment> QueryAsync(int id);

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="type">获取类型</param>
        /// <param name="id">壁纸Id或用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回评论列表</returns>
        Task<List<T_Comment>> QueryAsync(string type, int id, int page, int pageSize);

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="entity">评论信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        Task<bool> InsertAsync(T_Comment entity);

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        Task<bool> DeleteAsync(int id);
    }
}
