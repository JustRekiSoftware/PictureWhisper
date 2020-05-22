using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    /// <summary>
    /// 举报理由数据仓库
    /// </summary>
    public class ReportReasonRepository : IReportReasonRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public ReportReasonRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据Id获取举报理由
        /// </summary>
        /// <param name="id">举报理由Id</param>
        /// <returns>举报理由描述</returns>
        public async Task<string> QueryAsync(short id)
        {
            var reportReason = await context.ReportReasons.FindAsync(id);

            return reportReason.RR_Info;
        }

        /// <summary>
        /// 获取举报理由列表
        /// </summary>
        /// <returns>返回举报理由列表</returns>
        public async Task<List<T_ReportReason>> QueryAsync()
        {
            return await context.ReportReasons.ToListAsync();
        }

        /// <summary>
        /// 添加举报理由
        /// </summary>
        /// <param name="entity">举报理由信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public async Task<bool> InsertAsync(T_ReportReason entity)
        {
            context.ReportReasons.Add(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 更新举报理由
        /// </summary>
        /// <param name="id">举报理由Id</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功返回true，否则返回false</returns>
        public async Task<bool> UpdateAsync(short id, JsonPatchDocument<T_ReportReason> jsonPatch)
        {
            var target = await context.ReportReasons.FindAsync(id);
            jsonPatch.ApplyTo(target);//应用更改
            context.Entry(target).State = EntityState.Modified;//标记为已修改
            try
            {
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 删除举报理由
        /// </summary>
        /// <param name="id">举报理由Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public async Task<bool> DeleteAsync(short id)
        {
            var entity = await context.ReportReasons.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            context.ReportReasons.Remove(entity);
            try
            {
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }
    }
}
