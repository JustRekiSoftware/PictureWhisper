using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    /// <summary>
    /// 关注数据仓库
    /// </summary>
    public class FollowRepository : IFollowRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public FollowRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据关注者Id和被关注者Id检查是否已关注
        /// </summary>
        /// <param name="followerId">关注者Id</param>
        /// <param name="followedId">被关注者Id</param>
        /// <returns>已关注返回true，否则返回false</returns>
        public async Task<bool> QueryAsync(int followerId, int followedId)
        {
            if (followerId == followedId)
            {
                return false;
            }
            var result = await context.Follows
                .Where(p => p.FLW_FollowerID == followerId && p.FLW_FollowedID == followedId)
                .FirstOrDefaultAsync();
            if (result == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取用户关注列表
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>获取成功返回用户列表，否则返回null</returns>
        public async Task<List<T_User>> QueryAsync(int id, int page, int pageSize)
        {
            if (page <= 0 && pageSize <= 0)
            {
                return null;
            }
            var followedIds = await context.Follows
                .Where(p => p.FLW_FollowerID == id)
                .OrderByDescending(p => p.FLW_Date)
                .Select(p => p.FLW_FollowedID)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = new List<T_User>();
            foreach (var followedId in followedIds)
            {
                var user = await context.Users.FindAsync(followedId);
                if (user != null && user.U_Status != (short)Status.已删除)
                {
                    result.Add(user);
                }
            }

            return result;
        }

        /// <summary>
        /// 添加关注信息
        /// </summary>
        /// <param name="entity">关注信息</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public async Task<bool> InsertAsync(T_Follow entity)
        {
            if (entity.FLW_FollowedID == entity.FLW_FollowerID)
            {
                return false;
            }
            var user = await context.Users.FindAsync(entity.FLW_FollowedID);
            if (user.U_Status == (short)Status.已删除
                || user.U_Status == (short)Status.已注销)
            {
                return false;
            }
            var follower = await context.Users.FindAsync(entity.FLW_FollowerID);
            follower.U_FollowedNum += 1;
            context.Entry(follower).State = EntityState.Modified;
            var followed = await context.Users.FindAsync(entity.FLW_FollowedID);
            followed.U_FollowerNum += 1;
            context.Entry(followed).State = EntityState.Modified;
            entity.FLW_Date = DateTime.Now;
            context.Follows.Add(entity);
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
        /// 取消关注
        /// </summary>
        /// <param name="followerId">关注者Id</param>
        /// <param name="followedId">被关注者Id</param>
        /// <returns>取消成功返回true，否则返回false</returns>
        public async Task<bool> DeleteAsync(int followerId, int followedId)
        {
            if (followerId == followedId)
            {
                return false;
            }
            var entity = await context.Follows
                .Where(p => p.FLW_FollowerID == followerId && p.FLW_FollowedID == followedId)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return false;
            }
            var follower = await context.Users.FindAsync(entity.FLW_FollowerID);
            follower.U_FollowedNum -= 1;
            context.Entry(follower).State = EntityState.Modified;
            var followed = await context.Users.FindAsync(entity.FLW_FollowedID);
            followed.U_FollowerNum -= 1;
            context.Entry(followed).State = EntityState.Modified;
            context.Follows.Remove(entity);
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
    }
}
