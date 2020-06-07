using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace PictureWhisper.Client.ViewModels
{
    /// <summary>
    /// 回复列表的ViewModel
    /// </summary>
    public class ReplyListViewModel : BindableBase
    {
        public ObservableCollection<ReplyDto> CommentReplys { get; set; }
        public ObservableCollection<ReplyDto> MessageReplys { get; set; }
        private int UserId { get; set; }

        public ReplyListViewModel()
        {
            CommentReplys = new ObservableCollection<ReplyDto>();
            MessageReplys = new ObservableCollection<ReplyDto>();
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
        }

        /// <summary>
        /// 获取评论回复
        /// </summary>
        /// <param name="id">评论Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public async Task GetCommentReplysAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                CommentReplys.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取回复列表
                var url = string.Format("{0}reply/comment/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, id, page, pageSize);
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var replys = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = replys.ToObject<List<T_Reply>>();
                if (result == null)
                {
                    return;
                }
                //补充显示信息
                foreach (var reply in result)
                {
                    url = HttpClientHelper.baseUrl + "user/" + reply.RPL_PublisherID;
                    response = await client.GetAsync(new Uri(url));
                    if (!response.IsSuccessStatusCode)
                    {
                        continue;
                    }
                    var userInfoDto = JObject.Parse(await response.Content.ReadAsStringAsync())
                        .ToObject<UserInfoDto>();
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + userInfoDto.U_Avatar;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.CommentReplys.Add(new ReplyDto
                    {
                        ReplyInfo = reply,
                        PublisherInfo = userInfoDto,
                        PublisherAvatar = image,
                        DeleteButtonVisibility = UserId == reply.RPL_PublisherID ?
                            Visibility.Visible : Visibility.Collapsed
                    });
                }
            }
        }

        /// <summary>
        /// 获取回复消息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public async Task GetMessageReplysAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                MessageReplys.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取回复列表
                var url = string.Format("{0}reply/message/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, id, page, pageSize);
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var replys = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = replys.ToObject<List<T_Reply>>();
                if (result == null)
                {
                    return;
                }
                //补充显示信息
                foreach (var reply in result)
                {
                    url = HttpClientHelper.baseUrl + "user/" + reply.RPL_PublisherID;
                    response = await client.GetAsync(new Uri(url));
                    if (!response.IsSuccessStatusCode)
                    {
                        continue;
                    }
                    var userInfoDto = JObject.Parse(await response.Content.ReadAsStringAsync())
                        .ToObject<UserInfoDto>();
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + userInfoDto.U_Avatar;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.MessageReplys.Add(new ReplyDto
                    {
                        ReplyInfo = reply,
                        PublisherInfo = userInfoDto,
                        PublisherAvatar = image
                    });
                }
            }
        }
    }
}
