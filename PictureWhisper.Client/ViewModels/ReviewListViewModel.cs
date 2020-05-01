using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Client.ViewModels
{
    public class ReviewListViewModel
    {
        public ObservableCollection<ReviewDto> Reviews { get; set; }

        public ReviewListViewModel()
        {
            Reviews = new ObservableCollection<ReviewDto>();
        }

        public async Task GetReviewsAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                Reviews.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}review/message/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, id, page, pageSize);
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var messages = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = messages.ToObject<List<T_Review>>();
                if (result == null)
                {
                    return;
                }
                foreach (var message in result)
                {
                    this.Reviews.Add(new ReviewDto
                    {
                        ReviewInfo = message,
                        ReviewDisplayText = await GetReviewDisplayTextAsync(message)
                    });
                }
            }
        }

        public async Task<string> GetReviewDisplayTextAsync(T_Review review)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var builder = new StringBuilder();
                var url = string.Empty;
                switch (review.RV_Type)
                {
                    case (short)ReviewType.壁纸审核:
                        url = HttpClientHelper.baseUrl + "wallpaper/" + review.RV_ReviewedID;
                        var responseWallpaper = await client.GetAsync(new Uri(url));
                        if (!responseWallpaper.IsSuccessStatusCode)
                        {
                            break;
                        }
                        var wallpaper = JObject.Parse(await responseWallpaper.Content.ReadAsStringAsync())
                            .ToObject<T_Wallpaper>();
                        builder.Append("你上传的壁纸: ");
                        builder.Append(wallpaper.W_Title);
                        builder.Append(review.RV_Result ? " 已通过审核。" : " 包含违规信息，未通过审核，已删除。");
                        break;
                    case (short)ReviewType.举报审核:
                        url = HttpClientHelper.baseUrl + "report/" + review.RV_ReviewedID;
                        var responseReport = await client.GetAsync(new Uri(url));
                        if (!responseReport.IsSuccessStatusCode)
                        {
                            break;
                        }
                        var report = JObject.Parse(await responseReport.Content.ReadAsStringAsync())
                            .ToObject<T_Report>();
                        builder.Append("你举报的");
                        builder.Append(Enum.GetName(typeof(ReportType), report.RPT_Type));
                        builder.Append(": ");
                        switch (report.RPT_Type)
                        {
                            case (short)ReportType.壁纸:
                                url = HttpClientHelper.baseUrl + "wallpaper/" + report.RPT_ReportedID;
                                var respWallpaper = await client.GetAsync(new Uri(url));
                                if (!respWallpaper.IsSuccessStatusCode)
                                {
                                    break;
                                }
                                var wallpaperInfo = JObject.Parse(await respWallpaper.Content.ReadAsStringAsync())
                                    .ToObject<T_Wallpaper>();
                                builder.Append(wallpaperInfo.W_Title);
                                break;
                            case (short)ReportType.评论:
                                url = HttpClientHelper.baseUrl + "comment/" + report.RPT_ReportedID;
                                var respComment = await client.GetAsync(new Uri(url));
                                if (!respComment.IsSuccessStatusCode)
                                {
                                    break;
                                }
                                var commentInfo = JObject.Parse(await respComment.Content.ReadAsStringAsync())
                                    .ToObject<T_Comment>();
                                builder.Append(commentInfo.C_Content);
                                break;
                            case (short)ReportType.回复:
                                url = HttpClientHelper.baseUrl + "reply/" + report.RPT_ReportedID;
                                var respReply = await client.GetAsync(new Uri(url));
                                if (!respReply.IsSuccessStatusCode)
                                {
                                    break;
                                }
                                var replyInfo = JObject.Parse(await respReply.Content.ReadAsStringAsync())
                                    .ToObject<T_Reply>();
                                builder.Append(replyInfo.RPL_Content);
                                break;
                            case (short)ReportType.用户:
                                url = HttpClientHelper.baseUrl + "user/" + report.RPT_ReportedID;
                                var respUser = await client.GetAsync(new Uri(url));
                                if (!respUser.IsSuccessStatusCode)
                                {
                                    break;
                                }
                                var userInfo = JObject.Parse(await respUser.Content.ReadAsStringAsync())
                                    .ToObject<UserInfoDto>();
                                builder.Append(userInfo.U_Name);
                                break;
                        }
                        builder.Append(review.RV_Result ? " 经审核，不含违规信息，不对其做出任何处理。"
                            : " 经审核，包含违规信息，已将其删除。");
                        break;
                    default:
                        break;
                }

                return builder.ToString();
            }
        }
    }
}
