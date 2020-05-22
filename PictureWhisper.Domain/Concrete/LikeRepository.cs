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
    /// 点赞数据仓库
    /// </summary>
    public class LikeRepository : ILikeRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public LikeRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据用户Id和壁纸Id检查是否点赞
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>已点赞返回true，否则返回false</returns>
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

        /// <summary>
        /// 添加点赞信息
        /// </summary>
        /// <param name="entity">点赞信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
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
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            await UpdateUserTag(entity.L_LikerID);//更新用户兴趣标签
            return true;
        }

        /// <summary>
        /// 取消点赞
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="wallpaperId">壁纸Id</param>
        /// <returns>取消成功返回true，否则返回false</returns>
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
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            await UpdateUserTag(userId);//更新用户兴趣标签
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
