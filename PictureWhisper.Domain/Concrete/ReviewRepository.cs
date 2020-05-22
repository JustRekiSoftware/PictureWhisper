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
    /// 审核数据仓库
    /// </summary>
    public class ReviewRepository : IReviewRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public ReviewRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 获取审核人员审核的记录
        /// </summary>
        /// <param name="id">审核人员Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回审核记录列表</returns>
        public async Task<List<T_Review>> QueryAsync(int id, int page, int pageSize)
        {
            return await context.Reviews
                .Where(p => p.RV_ReviewerID == id)
                .OrderBy(p => p.RV_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        /// <summary>
        /// 添加审核记录
        /// </summary>
        /// <param name="entity">审核记录</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public async Task<bool> InsertAsync(T_Review entity)
        {
            T_Wallpaper wallpaper = null;
            T_Report report = null;
            switch (entity.RV_Type)
            {
                case (short)ReviewType.壁纸审核:
                    wallpaper = await context.Wallpapers.FindAsync(entity.RV_ReviewedID);
                    wallpaper.W_Status = (short)(entity.RV_Result ? Status.正常 : Status.已删除);
                    if (!entity.RV_Result)//审核未通过
                    {
                        await DeleteWallpaperAsync(wallpaper.W_ID);//删除壁纸下的评论、回复
                    }
                    context.Entry(wallpaper).State = EntityState.Modified;
                    break;
                case (short)ReviewType.举报审核:
                    report = await context.Reports.FindAsync(entity.RV_ReviewedID);
                    report.RPT_Status = (short)Status.正常;
                    context.Entry(report).State = EntityState.Modified;
                    switch (report.RPT_Type)
                    {
                        case (short)ReportType.壁纸://壁纸举报的处理
                            var reportedWallpaper = await context.Wallpapers.FindAsync(entity.RV_ReviewedID);
                            reportedWallpaper.W_Status = (short)(entity.RV_Result ? Status.正常 : Status.已删除);
                            if (!entity.RV_Result)
                            {
                                await DeleteWallpaperAsync(reportedWallpaper.W_ID);
                            }
                            context.Entry(reportedWallpaper).State = EntityState.Modified;
                            break;
                        case (short)ReportType.评论://评论举报的处理
                            var reportedComment = await context.Comments.FindAsync(entity.RV_ReviewedID);
                            reportedComment.C_Status = (short)(entity.RV_Result ? Status.正常 : Status.已删除);
                            if (!entity.RV_Result)
                            {
                                await DeleteCommentAsync(reportedComment.C_ID);
                            }
                            context.Entry(reportedComment).State = EntityState.Modified;
                            break;
                        case (short)ReportType.回复://回复举报的处理
                            var reportedReply = await context.Replies.FindAsync(entity.RV_ReviewedID);
                            reportedReply.RPL_Status = (short)(entity.RV_Result ? Status.正常 : Status.已删除);
                            context.Entry(reportedReply).State = EntityState.Modified;
                            break;
                        case (short)ReportType.用户://用户举报的处理
                            var reportedUser = await context.Users.FindAsync(entity.RV_ReviewedID);
                            reportedUser.U_Status = (short)(entity.RV_Result ? Status.正常 : Status.已删除);
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
            context.Reviews.Add(entity);//添加审核信息
            try
            {
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            if (wallpaper != null)
            {
                ReviewHelper.RemoveWallpaper(wallpaper);//将壁纸移出正在审核列表
            }
            if (report != null)
            {
                ReviewHelper.RemoveReport(report);//将举报移出正在处理列表
            }
            return true;
        }

        /// <summary>
        /// 删除壁纸下的评论、回复
        /// </summary>
        /// <param name="id">壁纸Id</param>
        /// <returns></returns>
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

        /// <summary>
        /// 删除评论下的回复
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns></returns>
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

        /// <summary>
        /// 删除用户关注信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns></returns>
        public async Task DeleteFollowAsync(long id)
        {
            var followers = await context.Follows.Where(p => p.FLW_FollowedID == id).ToListAsync();
            context.Follows.RemoveRange(followers);
            var followeds = await context.Follows.Where(p => p.FLW_FollowerID == id).ToListAsync();
            context.Follows.RemoveRange(followeds);
        }

        /// <summary>
        /// 获取审核通知消息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回审核记录列表</returns>
        public async Task<List<T_Review>> GetReviewMessageAsync(int id, int page, int pageSize)
        {
            return await context.Reviews
                .Where(p => p.RV_MsgToReportedID == id || p.RV_MsgToReporterID == id)
                .OrderBy(p => p.RV_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
