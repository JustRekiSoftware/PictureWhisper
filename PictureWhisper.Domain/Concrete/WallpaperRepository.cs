using System;
using System.Collections.Generic;
using System.Text;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using Microsoft.AspNetCore.JsonPatch;

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
                .Where(p => (p.W_Status == (short)Status.未审核 
                    || p.W_Status == (short)Status.正常)
                    && p.W_Type == type)
                .OrderByDescending(p => p.W_Date)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<T_Wallpaper>> GetRecommendWallpaperAsync(int id, int count)
        {
            return await context.Wallpapers.Take(count).ToListAsync();
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
            var top = await context.Wallpapers.MaxAsync(p => p.W_LikeNum + p.W_FavoriteNum * 2);
            var result = await context.Wallpapers.Where(p => p.W_Date > yestoday && p.W_Date < today
                && p.W_Status == (short)Status.正常
                && (p.W_LikeNum + p.W_FavoriteNum * 2) == top).FirstOrDefaultAsync();

            return result.W_Location;
        }
    }
}
