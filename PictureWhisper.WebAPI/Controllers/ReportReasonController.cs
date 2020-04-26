using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;

namespace PictureWhisper.WebAPI.Controllers
{
    [Route("api/report/reason")]
    [Authorize]
    [ApiController]
    public class ReportReasonController : ControllerBase
    {
        private IReportReasonRepository reportReasonRepo;

        public ReportReasonController(IReportReasonRepository repo)
        {
            reportReasonRepo = repo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetReportReasonAsync(short id)
        {
            var result = await reportReasonRepo.QueryAsync(id);
            if (result == null || result == string.Empty)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet]
        public async Task<ActionResult<List<T_ReportReason>>> GetReportReasonsAsync()
        {
            var result = await reportReasonRepo.QueryAsync();
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> PostReportReasonAsync(T_ReportReason entity)
        {
            var result = await reportReasonRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchReportReasonAsync(short id, 
            [FromBody] JsonPatchDocument<T_ReportReason> jsonPatch)
        {
            var result = await reportReasonRepo.UpdateAsync(id, jsonPatch);
            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReportReasonAsync(short id)
        {
            var result = await reportReasonRepo.DeleteAsync(id);

            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}