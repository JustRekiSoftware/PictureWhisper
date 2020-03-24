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
    [Route("api/review")]
    [Authorize]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private IReviewRepository reviewRepo;

        public ReviewController(IReviewRepository repo)
        {
            reviewRepo = repo;
        }

        [HttpGet("{id}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Review>>> GetReviewsAsync(int id, int page, int pageSize)
        {
            var result = await reviewRepo.QueryAsync(id, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("message/{id}/{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Review>>> GetReviewMessageAsync(int id, int page, int pageSize)
        {
            var result = await reviewRepo.GetReviewMessageAsync(id, page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> PostReviewAsync(T_Review entity)
        {
            var result = await reviewRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}