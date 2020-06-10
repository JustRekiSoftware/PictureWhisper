using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    /// <summary>
    /// 用户数据仓库接口
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 根据Id获取用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>获取成功返回用户信息，否则返回null</returns>
        Task<T_User> QueryAsync(int id);

        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="queryData">搜索关键字</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>返回用户列表</returns>
        Task<List<T_User>> QueryAsync(string queryData, int page, int pageSize);

        /// <summary>
        /// 检查邮箱是否已注册
        /// </summary>
        /// <param name="email">注册邮箱</param>
        /// <returns>已注册返回true，否则返回false</returns>
        Task<bool> CheckEmailAsync(string email);

        /// <summary>
        /// 检查用户名是否已注册
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>已注册返回true，否则返回false</returns>
        Task<bool> CheckNameAsync(string name);

        /// <summary>
        /// 用户登录检查
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="password">密码</param>
        /// <returns>登录成功返回登录信息，否则返回null</returns>
        Task<UserSigninDto> CheckSigninAsync(string email, string password);

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="entity">用户信息</param>
        /// <returns>注册成功返回登录信息，否则返回null</returns>
        Task<UserSigninDto> InsertAsync(T_User entity);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功返回true，否则返回false</returns>
        Task<bool> UpdateAsync(int id, JsonPatchDocument<T_User> jsonPatch);

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="code">验证码</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功返回true，否则返回false</returns>
        Task<bool> UpdatePasswordAsync(int id, string code,  JsonPatchDocument<T_User> jsonPatch);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// 发送密码修改验证码
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="email">邮箱</param>
        Task<int> SendIdentifyCodeAsync(int id, string email);
    }
}
