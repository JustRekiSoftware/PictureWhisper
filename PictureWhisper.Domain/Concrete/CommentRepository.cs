using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    /// <summary>
    /// 评论数据仓库
    /// </summary>
    public class CommentRepository : ICommentRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public CommentRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据Id获取评论
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns>返回评论</returns>
        public async Task<T_Comment> QueryAsync(int id)
        {
            return await context.Comments.FindAsync(id);
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="type">获取类型</param>
        /// <param name="id">壁纸Id或用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回评论列表</returns>
        public async Task<List<T_Comment>> QueryAsync(string type, int id, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            switch (type)
            {
                case "wallpaper"://获取壁纸评论
                    return await context.Comments
                        .Where(p => p.C_WallpaperID == id && p.C_Status == (short)Status.正常)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                case "message"://获取评论消息
                    return await context.Comments
                        .Where(p => p.C_ReceiverID == id && p.C_Status == (short)Status.正常)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                default:
                    return null;
            }
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="entity">评论信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public async Task<bool> InsertAsync(T_Comment entity)
        {
            var wallpaper = await context.Wallpapers.FindAsync(entity.C_WallpaperID);
            if (wallpaper.W_Status == (short)Status.已删除)
            {
                return false;
            }
            entity.C_Date = DateTime.Now;
            context.Comments.Add(entity);
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

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await context.Comments.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            var replies = await context.Replies.Where(p => p.RPL_CommentID == entity.C_ID 
                && p.RPL_Status == (short)Status.正常).ToListAsync();
            foreach (var reply in replies)
            {
                reply.RPL_Status = (short)Status.已删除;
                context.Entry(reply).State = EntityState.Modified;
            }
            entity.C_Status = (short)Status.已删除;//将评论修改为已删除
            context.Entry(entity).State = EntityState.Modified;//标记为已修改
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
