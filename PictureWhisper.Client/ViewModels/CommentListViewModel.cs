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
    public class CommentListViewModel
    {
        public ObservableCollection<CommentDto> WallpaperComments { get; set; }

        public ObservableCollection<CommentDto> MessageComments { get; set; }

        private int UserId { get; set; }

        public CommentListViewModel()
        {
            WallpaperComments = new ObservableCollection<CommentDto>();
            MessageComments = new ObservableCollection<CommentDto>();
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
        }

        public async Task GetWallpaperCommentsAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                WallpaperComments.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}comment/wallpaper/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, id, page, pageSize);
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var comments = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = comments.ToObject<List<T_Comment>>();
                if (result == null)
                {
                    return;
                }
                foreach (var comment in result)
                {
                    url = HttpClientHelper.baseUrl + "user/" + comment.C_PublisherID;
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
                    this.WallpaperComments.Add(new CommentDto
                    {
                        CommentInfo = comment,
                        PublisherInfo = userInfoDto,
                        PublisherAvatar = image,
                        DeleteButtonVisibility = UserId == comment.C_PublisherID ?
                            Visibility.Visible : Visibility.Collapsed,
                        AllReplyHyperlinkButtonDisplayText = comment.C_ReplyNum > 0 ? 
                            "查看" + comment.C_ReplyNum + "条回复" : "回复"
                    });
                }
            }
        }

        public async Task GetMessageCommentsAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                MessageComments.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}comment/message/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, id, page, pageSize);
                var response = await client.GetAsync(new Uri(url));
                var comments = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = comments.ToObject<List<T_Comment>>();
                if (result == null)
                {
                    return;
                }
                foreach (var comment in result)
                {
                    url = HttpClientHelper.baseUrl + "user/" + comment.C_PublisherID;
                    response = await client.GetAsync(new Uri(url));
                    var userInfoDto = JObject.Parse(await response.Content.ReadAsStringAsync())
                        .ToObject<UserInfoDto>();
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + userInfoDto.U_Avatar;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.MessageComments.Add(new CommentDto
                    {
                        CommentInfo = comment,
                        PublisherInfo = userInfoDto,
                        PublisherAvatar = image
                    });
                }
            }
        }
    }
}
