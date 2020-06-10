using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using PictureWhisper.Domain.Helper;
using PictureWhisper.Domain.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    /// <summary>
    /// 用户数据仓库
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public UserRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据Id获取用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>获取成功返回用户信息，否则返回null</returns>
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

        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="queryData">搜索关键字</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回用户列表</returns>
        public async Task<List<T_User>> QueryAsync(string queryData, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            var query = context.Users.AsQueryable();
            var keywords = queryData.Split(' ').ToList();
            IQueryable<T_User> keywordResult = null;
            foreach (var keyword in keywords)//匹配搜索关键字
            {
                var tmp = query.Where(p => p.U_Name.Contains(keyword));
                if (keywordResult == null)
                {
                    keywordResult = tmp;
                }
                keywordResult = keywordResult.Union(tmp);
            }
            query = keywordResult;

            return await query.Where(p => p.U_Status == (short)Status.正常)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        /// <summary>
        /// 检查邮箱是否已注册
        /// </summary>
        /// <param name="email">注册邮箱</param>
        /// <returns>已注册返回true，否则返回false</returns>
        public async Task<bool> CheckEmailAsync(string email)
        {
            var result = await context.Users.Where(p => p.U_Email == email).ToListAsync();
            if (result == null || result.Count != 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查用户名是否已注册
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>已注册返回true，否则返回false</returns>
        public async Task<bool> CheckNameAsync(string name)
        {
            var result = await context.Users.Where(p => p.U_Name == name).ToListAsync();
            if (result == null || result.Count != 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 用户登录检查
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="password">密码</param>
        /// <returns>登录成功返回登录信息，否则返回null</returns>
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

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="entity">用户信息</param>
        /// <returns>注册成功返回登录信息，否则返回null</returns>
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

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功返回true，否则返回false</returns>
        public async Task<bool> UpdateAsync(int id, JsonPatchDocument<T_User> jsonPatch)
        {
            var target = await context.Users.FindAsync(id);
            if (jsonPatch.Operations.Count(p => p.path == "U_Password") > 0)//修改密码
            {
                return false;//该更新方法不支持密码更新
            }
            jsonPatch.ApplyTo(target);//应用更新
            context.Entry(target).State = EntityState.Modified;//标记为已修改
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
        /// 更新用户密码
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="code">验证码</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功返回true，否则返回false</returns>
        public async Task<bool> UpdatePasswordAsync(int id, string code, JsonPatchDocument<T_User> jsonPatch)
        {
            var target = await context.Users.FindAsync(id);
            if (IdentityCodeHelper.IdentityCodes.ContainsKey(id))
            {
                if (IdentityCodeHelper.IdentityCodes[id].Item1 != code)//验证码不正确
                {
                    return false;
                }
                IdentityCodeHelper.RemoveIdentityCode(id);
            }
            jsonPatch.ApplyTo(target);//应用更新
            context.Entry(target).State = EntityState.Modified;//标记为已修改
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
        /// 删除用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await context.Users.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            entity.U_Status = (short)Status.已注销;//状态修改为已注销
            context.Entry(entity).State = EntityState.Modified;//标记为已修改
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
        /// 发送密码修改验证码
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="email">邮箱</param>
        /// <returns>有该用户则返回用户Id和验证码，无该用户则返回null</returns>
        public async Task<int> SendIdentifyCodeAsync(int id, string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(p => p.U_Email == email);
            if (user != null)
            {
                if (id != 0)
                {
                    var tmp = await context.Users.FindAsync(id);
                    if (tmp.U_Email != email || user.U_ID != id)
                    {
                        return 0;
                    }
                }
                var code = await MailHelper.SendIdentifyCodeAsync(user.U_Name, email);
                IdentityCodeHelper.AddIdentityCode(user.U_ID, code);
                return user.U_ID;
            }

            return 0;
        }
    }
}
