using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    public class ReplyRepository : IReplyRepository
    {
        private DB_PictureWhisperContext context;

        public ReplyRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        public async Task<List<T_Reply>> QueryAsync(string type, int id, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            switch (type)
            {
                case "comment":
                    return await context.Replies
                        .Where(p => p.RPL_CommentID == id && p.RPL_Status == (short)Status.正常)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                case "message":
                    return await context.Replies
                        .Where(p => p.RPL_ReceiverID == id && p.RPL_Status == (short)Status.正常)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                default:
                    return null;
            }
        }

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
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

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
            entity.RPL_Status = (short)Status.已删除;
            context.Entry(entity).State = EntityState.Modified;
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
    }
}
