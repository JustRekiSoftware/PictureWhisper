using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    public class CommentRepository : ICommentRepository
    {
        private DB_PictureWhisperContext context;

        public CommentRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        public async Task<T_Comment> QueryAsync(int id)
        {
            return await context.Comments.FindAsync(id);
        }

        public async Task<List<T_Comment>> QueryAsync(string type, int id, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            switch (type)
            {
                case "wallpaper":
                    return await context.Comments
                        .Where(p => p.C_WallpaperID == id && p.C_Status == (short)Status.正常)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                case "message":
                    return await context.Comments
                        .Where(p => p.C_ReceiverID == id && p.C_Status == (short)Status.正常)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                default:
                    return null;
            }
        }

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
            entity.C_Status = (short)Status.已删除;
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
