using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 举报理由数据仓库接口
    /// </summary>
    public interface IReportReasonRepository
    {
        /// <summary>
        /// 根据Id获取举报理由
        /// </summary>
        /// <param name="id">举报理由Id</param>
        /// <returns>举报理由描述</returns>
        Task<string> QueryAsync(short id);

        /// <summary>
        /// 获取举报理由列表
        /// </summary>
        /// <returns>返回举报理由列表</returns>
        Task<List<T_ReportReason>> QueryAsync();

        /// <summary>
        /// 添加举报理由
        /// </summary>
        /// <param name="entity">举报理由信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        Task<bool> InsertAsync(T_ReportReason entity);

        /// <summary>
        /// 更新举报理由
        /// </summary>
        /// <param name="id">举报理由Id</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功返回true，否则返回false</returns>
        Task<bool> UpdateAsync(short id, JsonPatchDocument<T_ReportReason> entity);

        /// <summary>
        /// 删除举报理由
        /// </summary>
        /// <param name="id">举报理由Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        Task<bool> DeleteAsync(short id);
    }
}
