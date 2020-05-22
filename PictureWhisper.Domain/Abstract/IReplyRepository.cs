using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 回复数据仓库接口
    /// </summary>
    public interface IReplyRepository
    {
        /// <summary>
        /// 获取回复列表
        /// </summary>
        /// <param name="type">获取类型</param>
        /// <param name="id">评论Id或用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>获取成功返回回复列表，否则返回null</returns>
        Task<List<T_Reply>> QueryAsync(string type, int id, int page, int pageSize);

        /// <summary>
        /// 添加回复
        /// </summary>
        /// <param name="entity">回复信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        Task<bool> InsertAsync(T_Reply entity);

        /// <summary>
        /// 删除回复
        /// </summary>
        /// <param name="id">回复Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        Task<bool> DeleteAsync(int id);
    }
}
