using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Controllers
{
    /// <summary>
    /// 举报控制器
    /// </summary>
    [Route("api/report")]
    [Authorize]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IReportRepository reportRepo;//举报数据仓库

        public ReportController(IReportRepository repo)
        {
            reportRepo = repo;
        }

        /// <summary>
        /// 根据Id获取举报记录
        /// </summary>
        /// <param name="id">举报Id</param>
        /// <returns>获取成功，则返回举报记录；否则返回404</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<T_Report>> GetReportAsync(int id)
        {
            var result = await reportRepo.QueryAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// 获取未处理举报记录列表
        /// </summary>
        /// <param name="userId">举报处理人员Id</param>
        /// <param name="count">获取数量</param>
        /// <returns>获取成功，则返回举报记录列表；否则返回404</returns>
        [HttpGet("unreviewed/{userId}/{count}")]
        public async Task<ActionResult<List<T_Report>>> GetUnReviewedReportsAsync(int userId, int count)
        {
            var result = await reportRepo.GetUnReviewedReportsAsync(userId, count);
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// 添加举报信息
        /// </summary>
        /// <param name="entity">举报信息</param>
        /// <returns>添加成功，则返回200；失败则返回404</returns>
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