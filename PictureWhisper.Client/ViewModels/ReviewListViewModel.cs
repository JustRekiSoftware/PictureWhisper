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
    /// <summary>
    /// 审核结果列表的ViewModel
    /// </summary>
    public class ReviewListViewModel
    {
        public ObservableCollection<ReviewDto> Reviews { get; set; }
        private int UserId { get; set; }

        public ReviewListViewModel()
        {
            Reviews = new ObservableCollection<ReviewDto>();
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
        }

        /// <summary>
        /// 获取审核结果
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public async Task GetReviewsAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                Reviews.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取审核结果
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
                result = result.Distinct(new ReviewEqualityComparer()).ToList();
                //补充显示信息
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

        /// <summary>
        /// 获取审核结果的显示信息
        /// </summary>
        /// <param name="review">审核结果</param>
        /// <returns>返回显示信息</returns>
        public async Task<string> GetReviewDisplayTextAsync(T_Review review)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var builder = new StringBuilder();
                var url = string.Empty;
                switch (review.RV_Type)
                {
                    case (short)ReviewType.壁纸审核://壁纸审核结果
                        url = HttpClientHelper.baseUrl + "wallpaper/" + review.RV_ReviewedID;
                        var responseWallpaper = await client.GetAsync(new Uri(url));
                        if (!responseWallpaper.IsSuccessStatusCode)
                        {
                            break;
                        }
                        var wallpaper = JObject.Parse(await responseWallpaper.Content.ReadAsStringAsync())
                            .ToObject<T_Wallpaper>();
                        //处理显示信息
                        builder.Append("你上传的壁纸: ");
                        builder.Append(wallpaper.W_Title);
                        builder.Append(review.RV_Result ? " 已通过审核。" : " 包含违规信息，未通过审核，已删除。");
                        break;
                    case (short)ReviewType.举报审核://举报审核结果
                        url = HttpClientHelper.baseUrl + "report/" + review.RV_ReviewedID;
                        var responseReport = await client.GetAsync(new Uri(url));
                        if (!responseReport.IsSuccessStatusCode)
                        {
                            break;
                        }
                        var report = JObject.Parse(await responseReport.Content.ReadAsStringAsync())
                            .ToObject<T_Report>();
                        //处理显示信息
                        if (review.RV_MsgToReporterID == review.RV_MsgToReportedID)
                        {
                            switch (report.RPT_Type)
                            {
                                case (short)ReportType.壁纸:
                                    builder.Append("你发布且举报的壁纸");
                                    break;
                                case (short)ReportType.评论:
                                    builder.Append("你发表且举报的评论");
                                    break;
                                case (short)ReportType.回复:
                                    builder.Append("你发表且举报的回复");
                                    break;
                                case (short)ReportType.用户:
                                    builder.Append("你的账号");
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (review.RV_MsgToReporterID == UserId)
                        {
                            builder.Append("你举报的");
                            builder.Append(Enum.GetName(typeof(ReportType), report.RPT_Type));
                        }
                        else if (review.RV_MsgToReportedID == UserId)
                        {
                            switch (report.RPT_Type)
                            {
                                case (short)ReportType.壁纸:
                                    builder.Append("你发布的壁纸");
                                    break;
                                case (short)ReportType.评论:
                                    builder.Append("你发表的评论");
                                    break;
                                case (short)ReportType.回复:
                                    builder.Append("你发表的回复");
                                    break;
                                case (short)ReportType.用户:
                                    builder.Append("你的账号");
                                    break;
                                default:
                                    break;
                            }
                        }
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

    /// <summary>
    /// 审核信息去重时的比较类
    /// </summary>
    public class ReviewEqualityComparer : IEqualityComparer<T_Review>
    {
        public bool Equals(T_Review x, T_Review y)
        {
            if (x.RV_ReviewedID == y.RV_ReviewedID)
            {
                return true;
            }
            else if (x.Equals(y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(T_Review obj)
        {
            var result = obj.RV_ReviewedID.ToString()
                + obj.RV_MsgToReportedID.ToString();

            return result.GetHashCode();
        }
    }

}
