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
    [Route("api/user")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository userRepo;

        public UserController(IUserRepository repo)
        {
            userRepo = repo;
        }

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

        [HttpGet("identify/{name}/{email}")]
        public async Task<ActionResult<string>> SendIdentifyCodeAsync(string name, string email)
        {
            var result = await MailHelper.SendIdentifyCodeAsync(name, email);

            return result;
        }
    }
}