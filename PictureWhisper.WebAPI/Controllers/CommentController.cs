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
    [Route("api/comment")]
    [Authorize]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private ICommentRepository commentRepo;

        public CommentController(ICommentRepository repo)
        {
            commentRepo = repo;
        }

        [HttpGet("{type}/{id}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Comment>>> GetCommentsAsync(string type,
            int id, int page, int pageSize)
        {
            var result = await commentRepo.QueryAsync(type, id, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> PostCommentAsync(T_Comment entity)
        {
            var result = await commentRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentAsync(int id)
        {
            var result = await commentRepo.DeleteAsync(id);

            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}