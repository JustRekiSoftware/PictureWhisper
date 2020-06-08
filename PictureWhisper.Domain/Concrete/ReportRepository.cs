using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using PictureWhisper.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    /// <summary>
    /// 举报数据仓库
    /// </summary>
    public class ReportRepository : IReportRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public ReportRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据Id获取举报信息
        /// </summary>
        /// <param name="id">举报Id</param>
        /// <returns>返回举报信息</returns>
        public async Task<T_Report> QueryAsync(int id)
        {
            return await context.Reports.FindAsync(id);
        }

        /// <summary>
        /// 获取未处理的举报信息
        /// </summary>
        /// <param name="userId">举报处理人员Id</param>
        /// <param name="count">获取数量</param>
        /// <returns></returns>
        public async Task<List<T_Report>> GetUnReviewedReportsAsync(int userId, int count)
        {
            var result = new List<T_Report>();
            var times = 0;
            while (result.Count < count)
            {
                if (times++ >= 3)
                {
                    break;
                }
                var tmp = await context.Reports
                    .Where(p => p.RPT_Status == (short)Status.未审核)
                    .OrderByDescending(p => p.RPT_Date)
                    .Skip(ReviewHelper.Reports.Count).Take(count).ToListAsync();
                foreach (var report in tmp)
                {
                    ReviewHelper.AddReport(ref result, report, userId);//将不是正在处理的举报信息加入返回列表
                }
            }

            return result;
        }

        /// <summary>
        /// 添加举报信息
        /// </summary>
        /// <param name="entity">举报信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public async Task<bool> InsertAsync(T_Report entity)
        {
            switch (entity.RPT_Type)//检查是否有必要保存举报信息
            {
                case (short)ReportType.壁纸:
                    var wallpaper = await context.Wallpapers.FindAsync(entity.RPT_ReportedID);
                    if (wallpaper.W_Status == (short)Status.已删除)
                    {
                        return false;
                    }
                    break;
                case (short)ReportType.评论:
                    var comment = await context.Comments.FindAsync(entity.RPT_ReportedID);
                    if (comment.C_Status == (short)Status.已删除)
                    {
                        return false;
                    }
                    break;
                case (short)ReportType.回复:
                    var reply = await context.Replies.FindAsync(entity.RPT_ReportedID);
                    if (reply.RPL_Status == (short)Status.已删除)
                    {
                        return false;
                    }
                    break;
                case (short)ReportType.用户:
                    var user = await context.Users.FindAsync(entity.RPT_ReportedID);
                    if (user.U_Status == (short)Status.已删除)
                    {
                        return false;
                    }
                    break;
            }
            entity.RPT_Date = DateTime.Now;
            entity.RPT_Status = (short)Status.未审核;
            context.Reports.Add(entity);
            try
            {
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }
    }
}
