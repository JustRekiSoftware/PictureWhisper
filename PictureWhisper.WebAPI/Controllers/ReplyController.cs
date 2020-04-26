using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;

namespace PictureWhisper.WebAPI.Controllers
{
    [Route("api/reply")]
    [Authorize]
    [ApiController]
    public class ReplyController : ControllerBase
    {
        private IReplyRepository replyRepo;

        public ReplyController(IReplyRepository repo)
        {
            replyRepo = repo;
        }

        [HttpGet("{type}/{id}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Reply>>> GetReplysAsync(string type,
            int id, int page, int pageSize)
        {
            var result = await replyRepo.QueryAsync(type, id, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> PostReplyAsync(T_Reply entity)
        {
            var result = await replyRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReplyAsync(int id)
        {
            var result = await replyRepo.DeleteAsync(id);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}