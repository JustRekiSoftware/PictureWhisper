using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWhisper.Client.ViewModels
{
    /// <summary>
    /// 举报列表的ViewModel
    /// </summary>
    public class ReportListViewModel
    {
        public ObservableCollection<ReportDto> Reports { get; set; }

        public ReportListViewModel()
        {
            Reports = new ObservableCollection<ReportDto>();
        }

        /// <summary>
        /// 获取举报列表
        /// </summary>
        /// <param name="count">获取数量</param>
        /// <returns></returns>
        public async Task GetReportsAsync(int count)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取举报列表
                var url = string.Format("{0}report/unreviewed/{1}",
                    HttpClientHelper.baseUrl, count);
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var reports = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = reports.ToObject<List<T_Report>>();
                if (result == null)
                {
                    return;
                }
                //补充显示信息
                foreach (var report in result)
                {
                    int messgaeToId = 0;
                    string displayText = string.Empty;
                    BitmapImage image = new BitmapImage();
                    switch (report.RPT_Type)
                    {
                        case (short)ReportType.壁纸:
                            url = HttpClientHelper.baseUrl
                                + "wallpaper/" + report.RPT_ReportedID;
                            response = await client.GetAsync(new Uri(url));
                            if (!response.IsSuccessStatusCode)
                            {
                                continue;
                            }
                            var wallpaper = JObject.Parse(
                                await response.Content.ReadAsStringAsync())
                                .ToObject<T_Wallpaper>();
                            messgaeToId = wallpaper.W_PublisherID;
                            displayText = GetWallpaperDisplayText(wallpaper);
                            url = HttpClientHelper.baseUrl
                                + "download/picture/origin/" + wallpaper.W_Location;
                            image = await ImageHelper.GetImageAsync(client, url);
                            break;
                        case (short)ReportType.评论:
                            url = HttpClientHelper.baseUrl
                                + "comment/" + report.RPT_ReportedID;
                            response = await client.GetAsync(new Uri(url));
                            if (!response.IsSuccessStatusCode)
                            {
                                continue;
                            }
                            var comment = JObject.Parse(
                                await response.Content.ReadAsStringAsync())
                                .ToObject<T_Comment>();
                            messgaeToId = comment.C_PublisherID;
                            displayText = GetCommentDisplayText(comment);
                            url = HttpClientHelper.baseUrl
                                + "download/picture/origin/" + await GetUserAvatarPath(comment.C_PublisherID);
                            image = await ImageHelper.GetImageAsync(client, url);
                            break;
                        case (short)ReportType.回复:
                            url = HttpClientHelper.baseUrl
                                + "reply/" + report.RPT_ReportedID;
                            response = await client.GetAsync(new Uri(url));
                            if (!response.IsSuccessStatusCode)
                            {
                                continue;
                            }
                            var reply = JObject.Parse(
                                await response.Content.ReadAsStringAsync())
                                .ToObject<T_Reply>();
                            messgaeToId = reply.RPL_PublisherID;
                            displayText = GetReplyDisplayText(reply);
                            url = HttpClientHelper.baseUrl
                                + "download/picture/origin/" + await GetUserAvatarPath(reply.RPL_PublisherID);
                            image = await ImageHelper.GetImageAsync(client, url);
                            break;
                        case (short)ReportType.用户:
                            url = HttpClientHelper.baseUrl
                                + "user/" + report.RPT_ReportedID;
                            response = await client.GetAsync(new Uri(url));
                            if (!response.IsSuccessStatusCode)
                            {
                                continue;
                            }
                            var user = JObject.Parse(
                                await response.Content.ReadAsStringAsync())
                                .ToObject<UserInfoDto>();
                            messgaeToId = user.U_ID;
                            displayText = GetUserDisplayText(user);
                            url = HttpClientHelper.baseUrl
                                + "download/picture/origin/" + user.U_Avatar;
                            image = await ImageHelper.GetImageAsync(client, url);
                            break;
                        default:
                            break;
                    }
                    url = HttpClientHelper.baseUrl
                        + "report/reason/" + report.RPT_Reason;
                    var reason = await client.GetStringAsync(new Uri(url));
                    this.Reports.Add(new ReportDto
                    {
                        ReportInfo = report,
                        Image = image,
                        DisplayText = displayText,
                        MessageToId = messgaeToId,
                        ReportReason = reason
                    });
                }
            }
        }

        /// <summary>
        /// 获取壁纸举报显示信息
        /// </summary>
        /// <param name="wallpaper">壁纸</param>
        /// <returns>返回显示信息</returns>
        private string GetWallpaperDisplayText(T_Wallpaper wallpaper)
        {
            StringBuilder builder = new StringBuilder(128);
            builder.AppendLine("#标签");
            builder.AppendLine(wallpaper.W_Tag);
            builder.AppendLine("#图语");
            builder.AppendLine(wallpaper.W_Story);

            return builder.ToString();
        }

        /// <summary>
        /// 获取评论举报显示信息
        /// </summary>
        /// <param name="comment">评论</param>
        /// <returns>返回显示信息</returns>
        private string GetCommentDisplayText(T_Comment comment)
        {
            StringBuilder builder = new StringBuilder(128);
            builder.AppendLine("#评论");
            builder.AppendLine(comment.C_Content);

            return builder.ToString();
        }

        /// <summary>
        /// 获取回复举报显示信息
        /// </summary>
        /// <param name="reply">回复</param>
        /// <returns>返回显示信息</returns>
        private string GetReplyDisplayText(T_Reply reply)
        {
            StringBuilder builder = new StringBuilder(128);
            builder.AppendLine("#回复");
            builder.AppendLine(reply.RPL_Content);

            return builder.ToString();
        }

        /// <summary>
        /// 获取用户举报显示信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>返回显示信息</returns>
        private string GetUserDisplayText(UserInfoDto user)
        {
            StringBuilder builder = new StringBuilder(128);
            builder.AppendLine("#昵称");
            builder.AppendLine(user.U_Name);
            builder.AppendLine("#简介");
            builder.AppendLine(user.U_Info);

            return builder.ToString();
        }

        /// <summary>
        /// 获取用户头像下载地址
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>返回用户头像下载地址</returns>
        private async Task<string> GetUserAvatarPath(int id)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "user/" + id;
                var response = await client.GetAsync(new Uri(url));
                var user = JObject.Parse(await response.Content.ReadAsStringAsync())
                    .ToObject<UserInfoDto>();

                return user.U_Avatar;
            }
        }
    }
}
