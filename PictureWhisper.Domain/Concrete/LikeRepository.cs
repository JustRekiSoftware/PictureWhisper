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

            await UpdateUserTag(entity.L_LikerID);
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

            await UpdateUserTag(userId);
            return true;
        }

        private async Task UpdateUserTag(int id)
        {
            var targetUser = await context.Users.FindAsync(id);
            var likeWallpaperIds = await context.Likes
                .Where(p => p.L_LikerID == id)
                .Select(p => p.L_WallpaperID).ToListAsync();
            var favoriteWallpaperIds = await context.Favorites
                .Where(p => p.FVRT_FavoritorID == id)
                .Select(p => p.FVRT_WallpaperID).ToListAsync();
            var wallpapersTagScores = new Dictionary<string, int>();
            var initalTags = targetUser.U_Tag.Split(' ').ToList();
            initalTags.RemoveAll(p => p == string.Empty);
            foreach (var tag in initalTags)
            {
                wallpapersTagScores.Add(tag, 75);
            }
            foreach (var wallpaperId in likeWallpaperIds)
            {
                var wallpaper = await context.Wallpapers.FindAsync(wallpaperId);
                var tags = wallpaper.W_Tag.Split(',').ToList();
                foreach (var tag in tags)
                {
                    if (wallpapersTagScores.ContainsKey(tag))
                    {
                        wallpapersTagScores[tag] += 1;
                    }
                    else
                    {
                        wallpapersTagScores.Add(tag, 1);
                    }
                }
            }
            foreach (var wallpaperId in favoriteWallpaperIds)
            {
                var wallpaper = await context.Wallpapers.FindAsync(wallpaperId);
                var tags = wallpaper.W_Tag.Split(',').ToList();
                foreach (var tag in tags)
                {
                    if (wallpapersTagScores.ContainsKey(tag))
                    {
                        wallpapersTagScores[tag] += 2;
                    }
                    else
                    {
                        wallpapersTagScores.Add(tag, 2);
                    }
                }
            }
            var orderedTagScores = wallpapersTagScores.OrderByDescending(p => p.Value);
            var resultTag = new StringBuilder();
            foreach (var keyValue in orderedTagScores)
            {
                if (resultTag.Length + keyValue.Key.Length >= 128)
                {
                    break;
                }
                resultTag.Append(" ");
                resultTag.Append(keyValue.Key);
            }

            targetUser.U_Tag = resultTag.ToString();
            context.Entry(targetUser).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }
        }
    }
}
