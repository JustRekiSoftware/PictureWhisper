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
    public class FollowRepository : IFollowRepository
    {
        private DB_PictureWhisperContext context;

        public FollowRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

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

        public async Task<List<T_User>> QueryAsync(int id, int page, int pageSize)
        {
            if (page <= 0 && pageSize <= 0)
            {
                return null;
            }
            var followedIds = await context.Follows
                .Where(p => p.FLW_FollowerID == id)
                .Select(p => p.FLW_FollowedID)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = new List<T_User>();
            foreach (var followedId in followedIds)
            {
                result.Add(await context.Users.FindAsync(followedId));
            }

            return result;
        }

        public async Task<bool> InsertAsync(T_Follow entity)
        {
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
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

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
