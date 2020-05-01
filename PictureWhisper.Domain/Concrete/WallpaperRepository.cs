using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    public class WallpaperRepository : IWallpaperRepository
    {
        private DB_PictureWhisperContext context;

        public WallpaperRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

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

        public async Task<List<T_Wallpaper>> QueryAsync(string queryData, 
            short filterData, string orderData, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            ParameterExpression parameter = Expression.Parameter(typeof(T_Wallpaper), "p");
            Expression temp = null;
            var keywords = queryData.Split(' ').ToList();
            foreach (var keyword in keywords)
            {
                MemberExpression memberTag = Expression.PropertyOrField(parameter, "W_Tag");
                MemberExpression memberStory = Expression.PropertyOrField(parameter, "W_Story");
                MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                ConstantExpression constant = Expression.Constant(keyword, typeof(string));
                if (temp == null)
                {
                    var left = Expression.Call(memberTag, method, constant);
                    var right = Expression.Call(memberStory, method, constant);
                    temp = Expression.Or(left, right);
                }
                else
                {
                    var left = Expression.Call(memberTag, method, constant);
                    var right = Expression.Call(memberStory, method, constant);
                    temp = Expression.And(temp, Expression.Or(left, right));
                }
            }
            if (filterData != 0)
            {
                MemberExpression memberType = Expression.PropertyOrField(parameter, "W_Type");
                ConstantExpression constatType = Expression.Constant(filterData, typeof(short));
                temp = Expression.And(temp, Expression.Equal(memberType, constatType));
            }
            MemberExpression memberStatus = Expression.PropertyOrField(parameter, "W_Status");
            ConstantExpression constatStatus = Expression.Constant((short)Status.未审核, typeof(short));
            var leftStatus = Expression.Equal(memberStatus, constatStatus);
            constatStatus = Expression.Constant((short)Status.正常, typeof(short));
            var rightStatus = Expression.Equal(memberStatus, constatStatus);
            temp = Expression.And(temp, Expression.Or(leftStatus, rightStatus));
            var lambdaWhere = Expression.Lambda<Func<T_Wallpaper, bool>>(temp, new[] { parameter });
            var orders = orderData.Split(' ').ToList();
            switch (orders[0])
            {
                case "date":
                    temp = Expression.PropertyOrField(parameter, "W_Date");
                    break;
                case "like":
                    temp = Expression.PropertyOrField(parameter, "W_LikeNum");
                    break;
                case "favorite":
                    temp = Expression.PropertyOrField(parameter, "W_FavoriteNum");
                    break;
            }
            if (orders[0] == "date")
            {
                var lambdaOrder = Expression.Lambda<Func<T_Wallpaper, DateTime>>(temp, new[] { parameter });

                return await context.Wallpapers.Where(lambdaWhere).OrderByDescending(lambdaOrder)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            else
            {
                var lambdaOrder = Expression.Lambda<Func<T_Wallpaper, int>>(temp, new[] { parameter });

                return await context.Wallpapers.Where(lambdaWhere).OrderByDescending(lambdaOrder)
                        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            }
        }

        public async Task<bool> InsertAsync(T_Wallpaper entity)
        {
            entity.W_LikeNum = 0;
            entity.W_FavoriteNum = 0;
            entity.W_Status = (short)Status.未审核;
            entity.W_Date = DateTime.Now;
            context.Wallpapers.Add(entity);
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
            var entity = await context.Wallpapers.FindAsync(id);
            if(entity == null)
            {
                return false;
            }
            var comments = await context.Comments.Where(p => p.C_WallpaperID == entity.W_ID
                && p.C_Status == (short)Status.正常).ToListAsync();
            foreach (var comment in comments)
            {
                var replies = await context.Replies.Where(p => p.RPL_CommentID == comment.C_ID
                    && p.RPL_Status == (short)Status.正常).ToListAsync();
                foreach (var reply in replies)
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
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        public async Task<List<T_Wallpaper>> GetTypeWallpaperAsync(short type, int page, int pageSize)
        {
            return await context.Wallpapers
                .Where(p => (p.W_Status != (short)Status.已删除)
                    && p.W_Type == type)
                .OrderByDescending(p => p.W_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<T_Wallpaper>> GetRecommendWallpaperAsync(int id, int count)
        {
            var rand = new Random();
            var result = new List<T_Wallpaper>();
            var userCount = await context.Users.CountAsync();
            var targetUser = await context.Users.FindAsync(id);
            var targetTags = targetUser.U_Tag.Split(' ').ToList();
            targetTags.RemoveAll(p => p == string.Empty);
            var tryCount = 0;
            while (result.Count < count)
            {
                if (tryCount++ > 3)
                {
                    break;
                }
                var wallpaperCount = await context.Wallpapers.CountAsync();
                if (wallpaperCount < 10000 || userCount < 1000)
                {
                    return await context.Wallpapers.Skip(rand.Next(0, wallpaperCount)).Take(count).ToListAsync();
                }
                var users = await context.Users.Skip(rand.Next(0, userCount)).Take(20).ToListAsync();
                if (users.Contains(targetUser))
                {
                    users.Remove(targetUser);
                }
                var scores = new List<double>();
                foreach (var user in users)
                {
                    var tags = user.U_Tag.Split(' ').ToList();
                    var score = (double)tags.Intersect(targetTags).Count() / (double)tags.Union(targetTags).Count();
                    scores.Add(score);
                }
                var maxScore = scores.Max();
                var neighbor = users[scores.IndexOf(maxScore)];
                var wallpaperIds =  await context.Favorites
                    .Where(p => p.FVRT_FavoritorID == neighbor.U_ID)
                    .OrderByDescending(p => p.FVRT_Date)
                    .Select(p => p.FVRT_WallpaperID).Take(count).ToListAsync();
                foreach (var wallpaperId in wallpaperIds)
                {
                    var wallpaper = await context.Wallpapers.FindAsync(wallpaperId);
                    if (wallpaper.W_Status == (short)Status.已删除 || result.Contains(wallpaper))
                    {
                        continue;
                    }
                    result.Add(wallpaper);
                }
            }

            return result;
        }

        public async Task<List<T_Wallpaper>> GetUnReviewedWallpaperAsync(int page, int pageSize)
        {
            return await context.Wallpapers
                .Where(p => p.W_Status == (short)Status.未审核)
                .OrderBy(p => p.W_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<T_Wallpaper>> GetPublishedWallpaperAsync(int id, int page, int pageSize)
        {
            return await context.Wallpapers
                .Where(p => p.W_PublisherID == id)
                .OrderBy(p => p.W_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

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

        public async Task<List<T_Wallpaper>> GetSpaceWallpaperAsync(int id, int page, int pageSize)
        {
            var result = new List<T_Wallpaper>();
            var userIds = await context.Follows
                .Where(p => p.FLW_FollowerID == id)
                .Select(p => p.FLW_FollowedID).ToListAsync();
            userIds.Add(id);
            foreach (var userId in userIds)
            {
                result.AddRange(await context.Wallpapers
                    .Where(p => p.W_PublisherID == userId)
                    .ToListAsync());
            }
            return result.OrderBy(p => p.W_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<string> GetTodayWallpaperAsync()
        {
            var now = DateTime.Now;
            var today = new DateTime(now.Year, now.Month, now.Day);
            var yestoday = new DateTime(now.Year, now.Month, now.Day - 1);
            var query = context.Wallpapers.Where(p => p.W_Date >= yestoday && p.W_Date <= today
                && p.W_Status == (short)Status.正常);
            var count = await query.CountAsync();
            if (count == 0)
            {
                return "today/" + yestoday.ToString("yyyy-MM-dd") + ".png";
            }
            var top = await query.MaxAsync(p => p.W_LikeNum + p.W_FavoriteNum * 2);
            var result = await query.Where(p => (p.W_LikeNum + p.W_FavoriteNum * 2) == top)
                .FirstOrDefaultAsync();

            return result.W_Location;
        }
    }
}
