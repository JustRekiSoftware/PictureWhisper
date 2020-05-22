using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using PictureWhisper.Domain.Helpers;

namespace PictureWhisper.WebAPI.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    [Route("api/user")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository userRepo;//用户数据仓库

        public UserController(IUserRepository repo)
        {
            userRepo = repo;
        }

        /// <summary>
        /// 根据Id获取用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>若获取到，则返回用户信息；没获取到，则返回404</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfoDto>> GetUserAsync(int id)
        {
            var result = await userRepo.QueryAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return new UserInfoDto
            {
                U_ID = result.U_ID,
                U_Name = result.U_Name,
                U_Info = result.U_Info,
                U_Avatar = result.U_Avatar,
                U_FollowerNum = result.U_FollowerNum,
                U_FollowedNum = result.U_FollowedNum
            };
        }

        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="queryData">搜索关键字</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>若获取到，则返回用户列表；没获取到，则返回404</returns>
        [HttpGet("query/{queryData}/{page}/{pageSize}")]
        public async Task<ActionResult<List<UserInfoDto>>> GetUsersAsync(string queryData, int page, int pageSize)
        {
            var result = await userRepo.QueryAsync(queryData, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result.Select(p => new UserInfoDto
            {
                U_ID = p.U_ID,
                U_Name = p.U_Name,
                U_Info = p.U_Info,
                U_Avatar = p.U_Avatar,
                U_FollowerNum = p.U_FollowerNum,
                U_FollowedNum = p.U_FollowedNum
            }).ToList();
        }

        /// <summary>
        /// 检查邮箱是否已注册
        /// </summary>
        /// <param name="email">注册邮箱</param>
        /// <returns>若已注册，则返回404；未注册则返回200</returns>
        [HttpGet("email/{email}")]
        public async Task<IActionResult> CheckEmailAsync(string email)
        {
            var result = await userRepo.CheckEmailAsync(email);
            if (result)
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// 检查用户名是否已注册
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>若已注册，则返回404；未注册则返回200</returns>
        [HttpGet("name/{name}")]
        public async Task<IActionResult> CheckNameAsync(string name)
        {
            var result = await userRepo.CheckNameAsync(name);
            if (result)
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="password">密码</param>
        /// <returns>若登录成功，则返回用户登录信息；失败则返回404</returns>
        [HttpGet("signin/{email}/{password}")]
        public async Task<ActionResult<UserSigninDto>> CheckSigninAsync(string email, string password)
        {
            var result = await userRepo.CheckSigninAsync(email, password);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userRegistDto">用户注册信息</param>
        /// <returns>注册成功，则返回用户登录信息；注册失败则返回404</returns>
        [HttpPost]
        public async Task<ActionResult<UserSigninDto>> PostUserAsync(UserRegistDto userRegistDto)
        {
            var entity = new T_User
            {
                U_Email = userRegistDto.U_Email,
                U_Name = userRegistDto.U_Name,
                U_Password = userRegistDto.U_Password,
                U_Tag = userRegistDto.U_Tag
            };
            var result = await userRepo.InsertAsync(entity);
            if (result != null)
            {
                return result;
            }

            return NotFound();
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功，则返回204；失败则返回404</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUserAsync(int id, [FromBody] JsonPatchDocument<T_User> jsonPatch)
        {
            var result = await userRepo.UpdateAsync(id, jsonPatch);
            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>删除成功，则返回200；删除失败则返回404</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var result = await userRepo.DeleteAsync(id);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// 发送密码修改验证码
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="email">邮箱</param>
        /// <returns>发送成功返回用户Id和验证码，否则返回404</returns>
        [HttpGet("identify/{id}/{email}")]
        public async Task<ActionResult<dynamic>> SendIdentifyCodeAsync(int id, string email)
        {
            var result = await userRepo.SendIdentifyCodeAsync(id, email);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }
    }
}