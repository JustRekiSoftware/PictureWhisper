using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 审核数据仓库接口
    /// </summary>
    public interface IReviewRepository
    {
        /// <summary>
        /// 获取审核人员审核的记录
        /// </summary>
        /// <param name="id">审核人员Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回审核记录列表</returns>
        Task<List<T_Review>> QueryAsync(int id, int page, int pageSize);

        /// <summary>
        /// 添加审核记录
        /// </summary>
        /// <param name="entity">审核记录</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        Task<bool> InsertAsync(T_Review entity);

        /// <summary>
        /// 获取审核通知消息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回审核记录列表</returns>
        Task<List<T_Review>> GetReviewMessageAsync(int id, int page, int pageSize);
    }
}
