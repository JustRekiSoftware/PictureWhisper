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
    /// 评论控制器
    /// </summary>
    [Route("api/comment")]
    [Authorize]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private ICommentRepository commentRepo;//评论数据仓库
        private IHubContext<NotifyHub> hubContext;//消息通知中心

        public CommentController(ICommentRepository repo, IHubContext<NotifyHub> context)
        {
            commentRepo = repo;
            hubContext = context;
        }

        /// <summary>
        /// 根据Id获取评论
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns>获取成功，则返回评论；失败，则返回404</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<T_Comment>> GetCommentsAsync(int id)
        {
            var result = await commentRepo.QueryAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="type">获取类型</param>
        /// <param name="id">壁纸Id或用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>获取成功，则返回评论列表；失败，则返回404</returns>
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

        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="entity">评论信息</param>
        /// <returns>发表成功，则通知消息并返回200；失败，则返回404</returns>
        [HttpPost]
        public async Task<IActionResult> PostCommentAsync(T_Comment entity)
        {
            var result = await commentRepo.InsertAsync(entity);
            if (result)
            {
                if (NotifyHub.ConnectionIdDict.ContainsKey(entity.C_ReceiverID))
                {
                    var connectionId = NotifyHub.ConnectionIdDict[entity.C_ReceiverID];
                    await hubContext.Clients.Client(connectionId)
                        .SendAsync("NotifyNewMessage", (short)NotifyMessageType.评论);
                }

                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <returns>删除成功，则返回200；失败，则返回404</returns>
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