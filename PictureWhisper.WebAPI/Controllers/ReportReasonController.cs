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
    /// <summary>
    /// 举报理由控制器
    /// </summary>
    [Route("api/report/reason")]
    [Authorize]
    [ApiController]
    public class ReportReasonController : ControllerBase
    {
        private IReportReasonRepository reportReasonRepo;//举报理由数据仓库

        public ReportReasonController(IReportReasonRepository repo)
        {
            reportReasonRepo = repo;
        }

        /// <summary>
        /// 根据Id获取举报理由
        /// </summary>
        /// <param name="id">举报理由Id</param>
        /// <returns>获取成功，则返回举报理由；失败则返回404</returns>
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

        /// <summary>
        /// 获取举报理由列表
        /// </summary>
        /// <returns>获取成功，则返回举报理由列表；失败则翻悔04</returns>
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

        /// <summary>
        /// 添加举报理由
        /// </summary>
        /// <param name="entity">举报理由信息</param>
        /// <returns>添加成功返回200；失败返回404</returns>
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

        /// <summary>
        /// 更新举报理由
        /// </summary>
        /// <param name="id">举报理由Id</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功，则返回204；失败则返回404</returns>
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

        /// <summary>
        /// 删除举报理由
        /// </summary>
        /// <param name="id">举报理由Id</param>
        /// <returns>删除成功返回200；否则返回404</returns>
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