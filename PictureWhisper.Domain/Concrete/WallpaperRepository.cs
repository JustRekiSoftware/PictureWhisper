using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Org.BouncyCastle.Asn1.Cms;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using PictureWhisper.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    /// <summary>
    /// 壁纸数据仓库
    /// </summary>
    public class WallpaperRepository : IWallpaperRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public WallpaperRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据Id获取壁纸
        /// </summary>
        /// <param name="id">壁纸Id</param>
        /// <returns>获取成功返回壁纸信息，失败返回null</returns>
        public async Task<T_Wallpaper> QueryAsync(int id)
        {
            var result = await context.Wallpapers.FindAsync(id);
            if (result == null)
            {
                return null;
            }
            if (result.W_Status == (short)Status.正常 
                || result.W_Status == (short)Status.未审核)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// 搜索壁纸
        /// </summary>
        /// <param name="queryData">搜索关键字</param>
        /// <param name="filterData">分区条件</param>
        /// <param name="orderData">排序条件</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>成功搜索到结果返回壁纸列表，否则返回null</returns>
        public async Task<List<T_Wallpaper>> QueryAsync(string queryData, 
            short filterData, string orderData, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            var query = context.Wallpapers.AsQueryable();
            var keywords = queryData.Split(' ').ToList();
            IQueryable<T_Wallpaper> keywordResult = null;
            foreach (var keyword in keywords)//匹配搜索关键字
            {
                var tmp = query.Where(p => p.W_Tag.Contains(keyword) || p.W_Title.Contains(keyword));
                if (keywordResult == null)
                {
                    keywordResult = tmp;
                }
                keywordResult = keywordResult.Union(tmp);
            }
            query = keywordResult;
            if (filterData != 0)//匹配分区
            {
                query = query.Where(p => p.W_Type == filterData);
            }
            query = query.Where(p => p.W_Status != (short)Status.已删除);
            switch (orderData)//匹配排序
            {
                case "date":
                    query = query.OrderByDescending(p => p.W_Date);
                    break;
                case "like":
                    query = query.OrderByDescending(p => p.W_LikeNum);
                    break;
                case "favorite":
                    query = query.OrderByDescending(p => p.W_FavoriteNum);
                    break;
            }

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        /// <summary>
        /// 添加壁纸
        /// </summary>
        /// <param name="entity">壁纸信息</param>
        /// <returns>添加成功返回true，失败则返回false</returns>
        public async Task<bool> InsertAsync(T_Wallpaper entity)
        {
            entity.W_LikeNum = 0;
            entity.W_FavoriteNum = 0;
            entity.W_Status = (short)Status.未审核;
            entity.W_Date = DateTime.Now;
            context.Wallpapers.Add(entity);
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
        /// 删除壁纸
        /// </summary>
        /// <param name="id">壁纸Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await context.Wallpapers.FindAsync(id);
            if(entity == null)
            {
                return false;
            }
            var comments = await context.Comments.Where(p => p.C_WallpaperID == entity.W_ID
                && p.C_Status == (short)Status.正常).ToListAsync();
            foreach (var comment in comments)//将该壁纸的所有评论标记为已删除
            {
                var replies = await context.Replies.Where(p => p.RPL_CommentID == comment.C_ID
                    && p.RPL_Status == (short)Status.正常).ToListAsync();
                foreach (var reply in replies)//将评论下的所有回复标记为已删除
                {
                    reply.RPL_Status = (short)Status.已删除;
                    context.Entry(reply).State = EntityState.Modified;
                }
                comment.C_Status = (short)Status.已删除;
                context.Entry(comment).State = EntityState.Modified;
            }
            entity.W_Status = (short)Status.已删除;
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

        /// <summary>
        /// 获取某分区壁纸
        /// </summary>
        /// <param name="type">分区Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回壁纸列表</returns>
        public async Task<List<T_Wallpaper>> GetTypeWallpaperAsync(short type, int page, int pageSize)
        {
            return await context.Wallpapers
                .Where(p => (p.W_Status != (short)Status.已删除)
                    && p.W_Type == type)
                .OrderByDescending(p => p.W_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        /// <summary>
        /// 获取推荐壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="count">获取数量</param>
        /// <returns>返回壁纸列表</returns>
        public async Task<List<T_Wallpaper>> GetRecommendWallpaperAsync(int id, int count)
        {
            var result = new List<T_Wallpaper>();
            result.AddRange(await GetNeighborRecommendAsync(id, count));//基于用户的协同过滤推荐
            var adWallpaper = await GetAdRecommendAsync(id);
            if (adWallpaper != null)
            {
                if (!result.Contains(adWallpaper))
                {
                    result.Add(adWallpaper);//添加广告壁纸
                }
            }
            if (result.Count < count)
            {
                var tmp = await GetRecentTopRecommendAsync(count - result.Count);
                result.AddRange(tmp);//获取最近热门壁纸
            }

            return result;
        }

        /// <summary>
        /// 获取邻近用户的收藏壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="count">获取数量</param>
        /// <returns>返回壁纸列表</returns>
        public async Task<List<T_Wallpaper>> GetNeighborRecommendAsync(int id, int count)
        {
            var rand = new Random();
            var result = new List<T_Wallpaper>();
            var userCount = await context.Users.CountAsync();
            var targetUser = await context.Users.FindAsync(id);
            var targetTags = targetUser.U_Tag.Split(',').ToList();//获取目标用户兴趣标签
            targetTags.RemoveAll(p => p == string.Empty);
            var users = await context.Users.Where(p => p.U_Type == (short)UserType.注册用户)
                .Skip(rand.Next(0, userCount - 15 <= 0 ? 1 : userCount - 15))
                .Take(15).ToListAsync();//随机获取15个用户
            if (users.Contains(targetUser))
            {
                users.Remove(targetUser);
            }
            var scores = new List<double>();
            foreach (var user in users)//计算邻近用户
            {
                var tags = user.U_Tag.Split(',').ToList();
                var score = (double)tags.Intersect(targetTags).Count() 
                    / (double)tags.Union(targetTags).Count();//计算用户间的Jaccard系数
                scores.Add(score);
            }
            var maxScore = scores.Max();
            var neighbor = users[scores.IndexOf(maxScore)];//获取邻近用户
            var wallpaperIds = await context.Favorites
                .Where(p => p.FVRT_FavoritorID == neighbor.U_ID)
                .OrderByDescending(p => p.FVRT_Date)
                .Select(p => p.FVRT_WallpaperID).Take(count).ToListAsync();//获取邻近用户的收藏壁纸
            foreach (var wallpaperId in wallpaperIds)
            {
                var wallpaper = await context.Wallpapers.FindAsync(wallpaperId);
                if (wallpaper.W_Status == (short)Status.已删除 || result.Contains(wallpaper))
                {
                    continue;
                }
                result.Add(wallpaper);
            }

            return result;
        }

        /// <summary>
        /// 获取广告壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>返回壁纸</returns>
        public async Task<T_Wallpaper> GetAdRecommendAsync(int id)
        {
            var rand = new Random();
            var targetUser = await context.Users.FindAsync(id);
            var targetTags = targetUser.U_Tag.Split(',').ToList();//获取目标用户兴趣标签
            targetTags.RemoveAll(p => p == string.Empty);
            var adWallpapers = await context.Wallpapers.Where(p => p.W_Tag.Contains("广告") 
                && p.W_Status != (short)Status.已删除).ToListAsync();
            if (adWallpapers.Count == 0)
            {
                return null;
            }
            adWallpapers = adWallpapers.Skip(rand.Next(0, adWallpapers.Count)).Take(15).ToList();//获取15个广告壁纸
            var scores = new List<double>();
            foreach (var wallpaper in adWallpapers)
            {
                var tags = wallpaper.W_Tag.Split(',').ToList();//获取壁纸标签
                tags.Remove("广告");
                var score = (double)tags.Intersect(targetTags).Count() 
                    / (double)tags.Union(targetTags).Count();//计算Jaccard系数
                scores.Add(score);
            }
            var maxScore = scores.Max();
            var result = adWallpapers[scores.IndexOf(maxScore)];//获取用户最感兴趣的广告壁纸

            return result;
        }

        /// <summary>
        /// 获取最近热门壁纸
        /// </summary>
        /// <param name="count">获取数量</param>
        /// <returns>返回壁纸列表</returns>
        public async Task<List<T_Wallpaper>> GetRecentTopRecommendAsync(int count)
        {
            var rand = new Random();
            var now = DateTime.Now;
            var today = new DateTime(now.Year, now.Month, now.Day);
            var yestoday = today.AddDays(-1);
            var resCount = 0;
            IQueryable<T_Wallpaper> query = context.Wallpapers.AsQueryable();
            while (resCount < count)//获取最近发布的壁纸
            {
                query = context.Wallpapers.Where(p => p.W_Date >= yestoday && p.W_Date <= today
                    && p.W_Status == (short)Status.正常);
                resCount = await query.CountAsync();
                yestoday = yestoday.AddDays(-1);
                var timeDiff = today - yestoday;
                if (timeDiff.TotalDays > 3)
                {
                    break;
                }
            }
            var result = await query.OrderByDescending(p => (p.W_LikeNum + p.W_FavoriteNum * 2))
                .Take(100).Skip(rand.Next(0, 100 - count)).Take(count).ToListAsync();//获取最近热门壁纸
            if (resCount < count)//若仍然不够，则从最新发布的10000个壁纸中获取热门壁纸
            {
                var tmp = await context.Wallpapers.OrderByDescending(p => p.W_Date).Take(10000)
                    .OrderByDescending(p => (p.W_LikeNum + p.W_FavoriteNum * 2)).ToListAsync();
                if (tmp.Count > count)
                {
                    tmp = tmp.Take(100).Skip(rand.Next(0, tmp.Count - count)).Take(count).ToList();
                }
                result.AddRange(tmp);
            }

            return result;
        }

        /// <summary>
        /// 获取未审核的壁纸
        /// </summary>
        /// <param name="count">获取数量</param>
        /// <returns>返回壁纸列表</returns>
        public async Task<List<T_Wallpaper>> GetUnReviewedWallpaperAsync(int count)
        {
            var result = new List<T_Wallpaper>();
            while (result.Count < count)
            {
                var tmp = await context.Wallpapers
                    .Where(p => p.W_Status == (short)Status.未审核)
                    .OrderByDescending(p => p.W_Date)
                    .Skip(ReviewHelper.Wallpapers.Count).Take(count).ToListAsync();
                foreach (var report in tmp)
                {
                    ReviewHelper.AddWallpaper(ref result, report);//将不是正在审核的壁纸加入返回列表
                }
            }

            return result;
        }

        /// <summary>
        /// 获取用户发布的壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回壁纸列表</returns>
        public async Task<List<T_Wallpaper>> GetPublishedWallpaperAsync(int id, int page, int pageSize)
        {
            return await context.Wallpapers
                .Where(p => p.W_PublisherID == id)
                .OrderBy(p => p.W_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        /// <summary>
        /// 获取用户收藏的壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回壁纸列表</returns>
        public async Task<List<T_Wallpaper>> GetFavoriteWallpaperAsync(int id, int page, int pageSize)
        {
            var result = new List<T_Wallpaper>();
            var wallpaperIds = await context.Favorites
                .Where(p => p.FVRT_FavoritorID == id)
                .Select(p => p.FVRT_WallpaperID).ToListAsync();
            foreach (var wallpaperId in wallpaperIds)
            {
                result.Add(await context.Wallpapers.FindAsync(wallpaperId));
            }
            return result.OrderBy(p => p.W_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 获取用户动态
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>壁纸列表</returns>
        public async Task<List<T_Wallpaper>> GetSpaceWallpaperAsync(int id, int page, int pageSize)
        {
            var result = new List<T_Wallpaper>();
            var userIds = await context.Follows
                .Where(p => p.FLW_FollowerID == id)
                .Select(p => p.FLW_FollowedID).ToListAsync();
            userIds.Add(id);
            foreach (var userId in userIds)//获取用户和关注的用户发布的壁纸
            {
                result.AddRange(await context.Wallpapers
                    .Where(p => p.W_PublisherID == userId)
                    .ToListAsync());
            }
            return result.OrderBy(p => p.W_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 获取今日壁纸
        /// </summary>
        /// <returns>返回壁纸下载路径</returns>
        public async Task<string> GetTodayWallpaperAsync()
        {
            var now = DateTime.Now;
            var today = new DateTime(now.Year, now.Month, now.Day);
            var yestoday = new DateTime(now.Year, now.Month, now.Day - 1);
            var query = context.Wallpapers.Where(p => p.W_Date >= yestoday && p.W_Date <= today
                && p.W_Status == (short)Status.正常);
            var count = await query.CountAsync();
            if (count == 0)//昨日没有人发布壁纸，则返回准备好的壁纸
            {
                return "today/" + yestoday.ToString("yyyy-MM-dd") + ".png";
            }
            var result = await query.OrderByDescending(p => (p.W_LikeNum + p.W_FavoriteNum * 2))
                .FirstOrDefaultAsync();//返回昨日的热门壁纸

            return result.W_Location;
        }
    }
}
