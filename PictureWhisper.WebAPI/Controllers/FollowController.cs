using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Controllers
{
    [Route("api/follow")]
    [Authorize]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private IFollowRepository followRepo;

        public FollowController(IFollowRepository repo)
        {
            followRepo = repo;
        }

        [HttpGet("{followerId}/{followedId}")]
        public async Task<ActionResult<bool>> CheckFollowAsync(int followerId, int followedId)
        {
            return await followRepo.QueryAsync(followerId, followedId);
        }

        [HttpGet("{id}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_User>>> GetFollowsAsync(int id, int page, int pageSize)
        {
            var result = await followRepo.QueryAsync(id, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> PostFollowAsync(T_Follow entity)
        {
            var result = await followRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{followerId}/{followedId}")]
        public async Task<IActionResult> DeleteFollowAsync(int followerId, int followedId)
        {
            var result = await followRepo.DeleteAsync(followerId, followedId);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}