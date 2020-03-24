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
    public class LikeRepository : ILikeRepository
    {
        private DB_PictureWhisperContext context;

        public LikeRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        public async Task<bool> QueryAsync(int userId, int wallpaperId)
        {
            var result = await context.Likes
                .Where(p => p.L_LikerID == userId && p.L_WallpaperID == wallpaperId)
                .FirstOrDefaultAsync();
            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> InsertAsync(T_Like entity)
        {
            var wallpaper = await context.Wallpapers.FindAsync(entity.L_WallpaperID);
            if (wallpaper.W_Status == (short)Status.已删除)
            {
                return false;
            }
            wallpaper.W_LikeNum += 1;
            context.Entry(wallpaper).State = EntityState.Modified;
            entity.L_Date = DateTime.Now;
            context.Likes.Add(entity);
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

        public async Task<bool> DeleteAsync(int userId, int wallpaperId)
        {
            var entity = await context.Likes
                .Where(p => p.L_LikerID == userId && p.L_WallpaperID == wallpaperId)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return false;
            }
            var wallpaper = await context.Wallpapers.FindAsync(entity.L_WallpaperID);
            wallpaper.W_LikeNum -= 1;
            context.Entry(wallpaper).State = EntityState.Modified;
            context.Likes.Remove(entity);
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
