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
    /// 回复数据仓库
    /// </summary>
    public class ReplyRepository : IReplyRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public ReplyRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 获取回复列表
        /// </summary>
        /// <param name="type">获取类型</param>
        /// <param name="id">评论Id或用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>获取成功返回回复列表，否则返回null</returns>
        public async Task<List<T_Reply>> QueryAsync(string type, int id, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            switch (type)
            {
                case "comment"://获取评论的回复
                    return await context.Replies
                        .Where(p => p.RPL_CommentID == id && p.RPL_Status == (short)Status.正常)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                case "message"://获取回复消息
                    return await context.Replies
                        .Where(p => p.RPL_ReceiverID == id && p.RPL_Status == (short)Status.正常)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                default:
                    return null;
            }
        }

        /// <summary>
        /// 添加回复
        /// </summary>
        /// <param name="entity">回复信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public async Task<bool> InsertAsync(T_Reply entity)
        {
            var comment = await context.Comments.FindAsync(entity.RPL_CommentID);
            if (comment.C_Status == (short)Status.已删除)
            {
                return false;
            }
            comment.C_ReplyNum += 1;
            context.Entry(comment).State = EntityState.Modified;
            entity.RPL_Date = DateTime.Now;
            context.Replies.Add(entity);
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
        /// 删除回复
        /// </summary>
        /// <param name="id">回复Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await context.Replies.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            var comment = await context.Comments.FindAsync(entity.RPL_CommentID);
            if (comment.C_Status == (short)Status.已删除)
            {
                return false;
            }
            comment.C_ReplyNum -= 1;
            context.Entry(comment).State = EntityState.Modified;
            entity.RPL_Status = (short)Status.已删除;//标记为已删除
            context.Entry(entity).State = EntityState.Modified;
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
