using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    public class ReportRepository : IReportRepository
    {
        private DB_PictureWhisperContext context;

        public ReportRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        public async Task<T_Report> QueryAsync(int id)
        {
            return await context.Reports.FindAsync(id);
        }

        public async Task<List<T_Report>> QueryAsync(int page, int pageSize)
        {
            return await context.Reports
                .Where(p => p.RPT_Status == (short)Status.未审核)
                .OrderByDescending(p => p.RPT_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<bool> InsertAsync(T_Report entity)
        {
            switch (entity.RPT_Type)
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
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }
    }
}
