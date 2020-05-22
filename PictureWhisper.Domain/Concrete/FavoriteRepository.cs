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
    /// <summary>
    /// 收藏数据仓库
    /// </summary>
    public class FavoriteRepository : IFavoriteRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public FavoriteRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据收藏者Id和壁纸Id检查是否已收藏
        /// </summary>
        /// <param name="favoritorId">收藏者Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>已收藏返回true，否则返回false</returns>
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

        /// <summary>
        /// 获取收藏列表
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>获取成功返回壁纸列表，否则返回null</returns>
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
                var wallpaper = await context.Wallpapers.FindAsync(wallpaperId);
                if (wallpaper != null && wallpaper.W_Status != (short)Status.已删除)
                {
                    result.Add(wallpaper);
                }
            }

            return result;
        }

        /// <summary>
        /// 添加收藏信息
        /// </summary>
        /// <param name="entity">收藏信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
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
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            await UpdateUserTag(entity.FVRT_FavoritorID);//更新用户兴趣标签
            return true;
        }

        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="favoritorId">收藏者Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>取消成功返回true，否则返回false</returns>
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
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            await UpdateUserTag(favoritorId);//更新用户兴趣标签
            return true;
        }

        /// <summary>
        /// 更新用户兴趣标签
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns></returns>
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
            var initalTags = targetUser.U_Tag.Split(',').ToList();//获取初始兴趣标签
            initalTags.RemoveAll(p => p == string.Empty);
            foreach (var tag in initalTags)
            {
                wallpapersTagScores.Add(tag, 10);//为初始兴趣标签添加初始得分
            }
            foreach (var wallpaperId in likeWallpaperIds)//统计点赞壁纸的标签得分
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
            foreach (var wallpaperId in favoriteWallpaperIds)//统计收藏壁纸的标签得分
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
            var orderedTagScores = wallpapersTagScores.OrderByDescending(p => p.Value);//根据得分排序
            var resultTag = new StringBuilder();
            foreach (var keyValue in orderedTagScores)//获取最终的用户兴趣标签
            {
                if (resultTag.Length + keyValue.Key.Length >= 128)
                {
                    break;
                }
                resultTag.Append(keyValue.Key);
                resultTag.Append(",");
            }
            resultTag.Remove(resultTag.Length - 1, 1);
            targetUser.U_Tag = resultTag.ToString();
            context.Entry(targetUser).State = EntityState.Modified;//标记为已修改
            try
            {
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateConcurrencyException)
            {

            }
        }
    }
}
