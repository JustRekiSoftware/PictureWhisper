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
    [Route("api/report")]
    [Authorize]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IReportRepository reportRepo;

        public ReportController(IReportRepository repo)
        {
            reportRepo = repo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<T_Report>> GetReportAsync(int id)
        {
            var result = await reportRepo.QueryAsync(id);
            if (result == null || result.RPT_Status != (short)Status.未审核)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("{page}/{pageSize}")]
        public async Task<ActionResult<List<T_Report>>> GetReportsAsync(int page, int pageSize)
        {
            var result = await reportRepo.QueryAsync(page, pageSize);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> PostReportAsync(T_Report entity)
        {
            var result = await reportRepo.InsertAsync(entity);
            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}