using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using PictureWhisper.WebAPI.Hubs;

namespace PictureWhisper.WebAPI.Controllers
{
    /// <summary>
    /// 审核控制器
    /// </summary>
    [Route("api/review")]
    [Authorize]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private IReviewRepository reviewRepo;//审核数据仓库
        private IHubContext<NotifyHub> hubContext;//消息通知中心

        public ReviewController(IReviewRepository repo, IHubContext<NotifyHub> context)
        {
            reviewRepo = repo;
            hubContext = context;
        }

        /// <summary>
        /// 根据审核人员Id获取审核记录列表
        /// </summary>
        /// <param name="id">审核人员Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>获取成功，则返回审核记录列表；失败则返回404</returns>
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

        /// <summary>
        /// 根据用户Id获取审核记录列表
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>获取成功，则返回审核记录列表；失败则返回404</returns>
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

        /// <summary>
        /// 审核壁纸或处理举报信息
        /// </summary>
        /// <param name="entity">审核信息</param>
        /// <returns>添加成功，则返回200；失败则返回404</returns>
        [HttpPost]
        public async Task<IActionResult> PostReviewAsync(T_Review entity)
        {
            var result = await reviewRepo.InsertAsync(entity);
            if (result)
            {
                if (NotifyHub.ConnectionIdCollect.ContainsKey(entity.RV_MsgToReportedID))
                {
                    var connectionId = NotifyHub.ConnectionIdCollect[entity.RV_MsgToReportedID];
                    await hubContext.Clients.Client(connectionId)
                        .SendAsync("NotifyNewMessage", (short)NotifyMessageType.回复);
                }
                if (NotifyHub.ConnectionIdCollect.ContainsKey(entity.RV_MsgToReporterID))
                {
                    var connectionId = NotifyHub.ConnectionIdCollect[entity.RV_MsgToReporterID];
                    await hubContext.Clients.Client(connectionId)
                        .SendAsync("NotifyNewMessage", (short)NotifyMessageType.回复);
                }

                return Ok();
            }

            return NotFound();
        }
    }
}