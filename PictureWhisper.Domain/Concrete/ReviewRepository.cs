using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    public class ReviewRepository : IReviewRepository
    {
        private DB_PictureWhisperContext context;

        public ReviewRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        public async Task<List<T_Review>> QueryAsync(int id, int page, int pageSize)
        {
            return await context.Reviews
                .Where(p => p.RV_ReviewerID == id)
                .OrderBy(p => p.RV_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<bool> InsertAsync(T_Review entity)
        {
            switch (entity.RV_Type)
            {
                case (short)ReviewType.壁纸审核:
                    var wallpaper = await context.Wallpapers.FindAsync(entity.RV_ReviewedID);
                    wallpaper.W_Status = (short)(entity.RV_Result ? Status.正常 : Status.已删除);
                    context.Entry(wallpaper).State = EntityState.Modified;
                    break;
                case (short)ReviewType.举报审核:
                    var report = await context.Reports.FindAsync(entity.RV_ReviewedID);
                    report.RPT_Status = (short)Status.正常;
                    context.Entry(report).State = EntityState.Modified;
                    switch (report.RPT_Type)
                    {
                        case (short)ReportType.壁纸:
                            var reportedWallpaper = await context.Wallpapers.FindAsync(entity.RV_ReviewedID);
                            reportedWallpaper.W_Status = (short)(entity.RV_Result ? Status.正常 : Status.已删除);
                            if (!entity.RV_Result)
                            {
                                await DeleteWallpaperAsync(reportedWallpaper.W_ID);
                            }
                            context.Entry(reportedWallpaper).State = EntityState.Modified;
                            break;
                        case (short)ReportType.评论:
                            var reportedComment = await context.Comments.FindAsync(entity.RV_ReviewedID);
                            reportedComment.C_Status = (short)(entity.RV_Result ? Status.正常 : Status.已删除);
                            if (!entity.RV_Result)
                            {
                                await DeleteCommentAsync(reportedComment.C_ID);
                            }
                            context.Entry(reportedComment).State = EntityState.Modified;
                            break;
                        case (short)ReportType.回复:
                            var reportedReply = await context.Replies.FindAsync(entity.RV_ReviewedID);
                            reportedReply.RPL_Status = (short)(entity.RV_Result ? Status.正常 : Status.已删除);
                            context.Entry(reportedReply).State = EntityState.Modified;
                            break;
                        case (short)ReportType.用户:
                            var reportedUser = await context.Users.FindAsync(entity.RV_ReviewedID);
                            reportedUser.U_Status = (short)(entity.RV_Result ? Status.正常 : Status.已删除);
                            if (!entity.RV_Result)
                            {
                                await DeleteFollowAsync(reportedUser.U_ID);
                            }
                            context.Entry(reportedUser).State = EntityState.Modified;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            entity.RV_Date = DateTime.Now;
            context.Reviews.Add(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        public async Task DeleteWallpaperAsync(int id)
        {
            var comments = await context.Comments.Where(p => p.C_WallpaperID == id
                && p.C_Status == (short)Status.正常).ToListAsync();
            foreach (var comment in comments)
            {
                var replies = await context.Replies.Where(p => p.RPL_CommentID == comment.C_ID
                    && p.RPL_Status == (short)Status.正常).ToListAsync();
                foreach (var reply in replies)
                {
                    reply.RPL_Status = (short)Status.已删除;
                    context.Entry(reply).State = EntityState.Modified;
                }
                comment.C_Status = (short)Status.已删除;
                context.Entry(comment).State = EntityState.Modified;
            }
        }

        public async Task DeleteCommentAsync(int id)
        {
            var replies = await context.Replies.Where(p => p.RPL_CommentID == id
                    && p.RPL_Status == (short)Status.正常).ToListAsync();
            foreach (var reply in replies)
            {
                reply.RPL_Status = (short)Status.已删除;
                context.Entry(reply).State = EntityState.Modified;
            }
        }

        public async Task DeleteFollowAsync(long id)
        {
            var follows = await context.Follows.Where(p => p.FLW_FollowedID == id).ToListAsync();
            context.Follows.RemoveRange(follows);
        }

        public async Task<List<T_Review>> GetReviewMessageAsync(int id, int page, int pageSize)
        {
            return await context.Reviews
                .Where(p => p.RV_MsgToReportedID == id || p.RV_MsgToReporterID == id)
                .OrderBy(p => p.RV_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
