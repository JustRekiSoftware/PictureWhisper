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
    public class FavoriteRepository : IFavoriteRepository
    {
        private DB_PictureWhisperContext context;

        public FavoriteRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        public async Task<bool> QueryAsync(int favoritorId, int wallpaperId)
        {
            var result = await context.Favorites
                .Where(p => p.FVRT_FavoritorID == favoritorId && p.FVRT_WallpaperID == wallpaperId)
                .FirstOrDefaultAsync();
            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<List<T_Wallpaper>> QueryAsync(int id, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            var wallpaperIds = await context.Favorites
                .Where(p => p.FVRT_FavoritorID == id)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(p => p.FVRT_WallpaperID).ToListAsync();
            var result = new List<T_Wallpaper>();
            foreach (var wallpaperId in wallpaperIds)
            {
                result.Add(await context.Wallpapers.FindAsync(wallpaperId));
            }

            return result;
        }

        public async Task<bool> InsertAsync(T_Favorite entity)
        {
            var wallpaper = await context.Wallpapers.FindAsync(entity.FVRT_WallpaperID);
            if (wallpaper.W_Status == (short)Status.已删除)
            {
                return false;
            }
            wallpaper.W_FavoriteNum += 1;
            context.Entry(wallpaper).State = EntityState.Modified;
            entity.FVRT_Date = DateTime.Now;
            context.Favorites.Add(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            await UpdateUserTag(entity.FVRT_FavoritorID);
            return true;
        }

        public async Task<bool> DeleteAsync(int favoritorId, int wallpaperId)
        {
            var entity = await context.Favorites
                .Where(p => p.FVRT_FavoritorID == favoritorId && p.FVRT_WallpaperID == wallpaperId)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return false;
            }
            var wallpaper = await context.Wallpapers.FindAsync(entity.FVRT_WallpaperID);
            wallpaper.W_FavoriteNum -= 1;
            context.Entry(wallpaper).State = EntityState.Modified;
            context.Favorites.Remove(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            await UpdateUserTag(favoritorId);
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
