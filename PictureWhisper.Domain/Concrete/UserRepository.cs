using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using PictureWhisper.Domain.Extensions;

namespace PictureWhisper.Domain.Concrete
{
    public class UserRepository : IUserRepository
    {
        private DB_PictureWhisperContext context;

        public UserRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        public async Task<T_User> QueryAsync(int id)
        {
            var result = await context.Users.FindAsync(id);
            if (result == null)
            {
                return null;
            }
            if (result.U_Status == (short)Status.正常)
            {
                return result;
            }

            return null;
        }

        public async Task<List<T_User>> QueryAsync(string queryData, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            ParameterExpression parameter = Expression.Parameter(typeof(T_User), "p");
            Expression temp = null;
            var keywords = queryData.Split(' ').ToList();
            foreach (var keyword in keywords)
            {
                MemberExpression member = Expression.PropertyOrField(parameter, "U_Name");
                MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                ConstantExpression constant = Expression.Constant(keyword, typeof(string));
                if (temp == null)
                {
                    temp = Expression.Call(member, method, constant);
                }
                else
                {
                    temp = Expression.And(temp, Expression.Call(member, method, constant));
                }
            }
            MemberExpression memberStatus = Expression.PropertyOrField(parameter, "U_Status");
            ConstantExpression constantStatus = Expression.Constant((short)Status.正常, typeof(short));
            temp = Expression.And(temp, Expression.Equal(memberStatus, constantStatus));
            var lambda = Expression.Lambda<Func<T_User, bool>>(temp, new[] { parameter });

            return await context.Users.Where(lambda).Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        }

        public async Task<bool> CheckEmailAsync(string email)
        {
            var result = await context.Users.Where(p => p.U_Email == email).ToListAsync();
            if (result == null || result.Count != 1)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CheckNameAsync(string name)
        {
            var result = await context.Users.Where(p => p.U_Name == name).ToListAsync();
            if (result == null || result.Count != 1)
            {
                return false;
            }

            return true;
        }

        public async Task<UserSigninDto> CheckSigninAsync(string email, string password)
        {
            var result = await context.Users.Where(p => p.U_Email == email
                && p.U_Password == password).ToListAsync();
            if (result == null || result.Count != 1)
            {
                return null;
            }

            return result.Select(p => new UserSigninDto
            {
                U_ID = p.U_ID,
                U_Avatar = p.U_Avatar,
                U_Type = p.U_Type,
                U_Status = p.U_Status
            }).FirstOrDefault();
        }

        public async Task<UserSigninDto> InsertAsync(T_User entity)
        {
            entity.U_Avatar = "default/avatar.png";
            entity.U_Info = "这个人很懒，什么也没有写";
            entity.U_FollowerNum = 0;
            entity.U_FollowedNum = 0;
            entity.U_Type = (short)UserType.注册用户;
            entity.U_Status = (short)Status.正常;
            context.Users.Add(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return null;
            }

            return new UserSigninDto
            {
                U_ID = entity.U_ID,
                U_Avatar = entity.U_Avatar,
                U_Type = entity.U_Type,
                U_Status = entity.U_Status
            };
        }

        public async Task<bool> UpdateAsync(int id, JsonPatchDocument<T_User> jsonPatch)
        {
            var target = await context.Users.FindAsync(id);
            jsonPatch.ApplyTo(target);
            context.Entry(target).State = EntityState.Modified;
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

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await context.Users.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            entity.U_Status = (short)Status.已注销;
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
    }
}
