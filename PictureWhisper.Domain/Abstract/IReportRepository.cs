using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 举报数据仓库接口
    /// </summary>
    public interface IReportRepository
    {
        /// <summary>
        /// 根据Id获取举报信息
        /// </summary>
        /// <param name="id">举报Id</param>
        /// <returns>返回举报信息</returns>
        Task<T_Report> QueryAsync(int id);

        /// <summary>
        /// 获取未处理的举报信息
        /// </summary>
        /// <param name="userId">举报处理人员Id</param>
        /// <param name="count">获取数量</param>
        /// <returns></returns>
        Task<List<T_Report>> GetUnReviewedReportsAsync(int userId, int count);

        /// <summary>
        /// 添加举报信息
        /// </summary>
        /// <param name="entity">举报信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        Task<bool> InsertAsync(T_Report entity);
    }
}
