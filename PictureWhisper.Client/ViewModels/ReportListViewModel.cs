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
    public class ReportListViewModel
    {
        public ObservableCollection<ReportDto> Reports { get; set; }

        public ReportListViewModel()
        {
            Reports = new ObservableCollection<ReportDto>();
        }

        public async Task GetReportsAsync(int page, int pageSize)
        {
            if (page == 1)
            {
                Reports.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}report/{1}/{2}",
                    HttpClientHelper.baseUrl, page, pageSize);
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

        private string GetWallpaperDisplayText(T_Wallpaper wallpaper)
        {
            StringBuilder builder = new StringBuilder(128);
            builder.AppendLine("标签");
            builder.AppendLine(wallpaper.W_Tag);
            builder.AppendLine("图语");
            builder.AppendLine(wallpaper.W_Story);

            return builder.ToString();
        }

        private string GetCommentDisplayText(T_Comment comment)
        {
            StringBuilder builder = new StringBuilder(128);
            builder.AppendLine("评论");
            builder.AppendLine(comment.C_Content);

            return builder.ToString();
        }

        private string GetReplyDisplayText(T_Reply reply)
        {
            StringBuilder builder = new StringBuilder(128);
            builder.AppendLine("回复");
            builder.AppendLine(reply.RPL_Content);

            return builder.ToString();
        }

        private string GetUserDisplayText(UserInfoDto user)
        {
            StringBuilder builder = new StringBuilder(128);
            builder.AppendLine("昵称");
            builder.AppendLine(user.U_Name);
            builder.AppendLine("简介");
            builder.AppendLine(user.U_Info);

            return builder.ToString();
        }

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
