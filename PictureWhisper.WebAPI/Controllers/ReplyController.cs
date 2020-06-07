using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using PictureWhisper.WebAPI.Hubs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.WebAPI.Controllers
{
    /// <summary>
    /// 回复控制器
    /// </summary>
    [Route("api/reply")]
    [Authorize]
    [ApiController]
    public class ReplyController : ControllerBase
    {
        private IReplyRepository replyRepo;//回复数据仓库
        private IHubContext<NotifyHub> hubContext;//消息通知中心

        public ReplyController(IReplyRepository repo, IHubContext<NotifyHub> context)
        {
            replyRepo = repo;
            hubContext = context;
        }

        /// <summary>
        /// 获取回复列表
        /// </summary>
        /// <param name="type">获取类型</param>
        /// <param name="id">评论Id或用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>获取成功，则返回回复列表；失败，则返回404</returns>
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

        /// <summary>
        /// 发表回复
        /// </summary>
        /// <param name="entity">回复信息</param>
        /// <returns>发表成功，则返回200；失败，则返回404</returns>
        [HttpPost]
        public async Task<IActionResult> PostReplyAsync(T_Reply entity)
        {
            var result = await replyRepo.InsertAsync(entity);
            if (result)
            {
                if (NotifyHub.ConnectionIdDict.ContainsKey(entity.RPL_ReceiverID))
                {
                    var connectionId = NotifyHub.ConnectionIdDict[entity.RPL_ReceiverID];
                    await hubContext.Clients.Client(connectionId)
                        .SendAsync("NotifyNewMessage", (short)NotifyMessageType.回复);
                }

                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// 删除回复
        /// </summary>
        /// <param name="id">回复Id</param>
        /// <returns>删除成功则返回200；失败，则返回404</returns>
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