using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace PictureWhisper.Client.ViewModels
{
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

        public async Task GetCommentReplysAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                CommentReplys.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
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

        public async Task GetMessageReplysAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                MessageReplys.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
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
