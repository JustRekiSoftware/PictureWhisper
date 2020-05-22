using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 回复消息页面
    /// </summary>
    public sealed partial class MessageReplyPage : Page
    {
        private ReplyListViewModel ReplyLVM { get; set; }
        private int UserId { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private ReplyDto ReplyTo { get; set; }

        public MessageReplyPage()
        {
            ReplyLVM = new ReplyListViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击举报按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplyReportButton_Click(object sender, RoutedEventArgs e)
        {
            var reply = (ReplyDto)((MenuFlyoutItem)sender).DataContext;
            var reportInfo = new T_Report();
            reportInfo.RPT_ReporterID = UserId;
            reportInfo.RPT_ReportedID = reply.ReplyInfo.RPL_ID;
            reportInfo.RPT_Type = (short)ReportType.回复;
            dynamic param = new
            {
                ReportInfo = reportInfo,
                MainPageName = "MessageMainPage"
            };
            MessageMainPage.PageFrame.Navigate(typeof(ReportPage), param);
        }

        /// <summary>
        /// 点击删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ReplyDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var reply = (ReplyDto)((MenuFlyoutItem)sender).DataContext;
            var contentDialog = new ContentDialog
            {
                Title = "删除回复",
                Content = "是否删除回复？",
                PrimaryButtonText = "是",
                SecondaryButtonText = "否"
            };
            contentDialog.PrimaryButtonClick += async (_sender, _e) =>
            {
                using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                {
                    var url = HttpClientHelper.baseUrl + "reply/" + reply.ReplyInfo.RPL_ID;
                    var resp = await client.DeleteAsync(new Uri(url));
                    if (resp.IsSuccessStatusCode)
                    {
                        ReplyLVM.CommentReplys.Remove(reply);
                    }
                    else
                    {
                        contentDialog.Hide();
                    }
                }
            };
            contentDialog.SecondaryButtonClick += (_sender, _e) =>
            {
                contentDialog.Hide();
            };
            await contentDialog.ShowAsync();
        }

        /// <summary>
        /// 点击回复超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ReplyHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            var reply = (ReplyDto)((HyperlinkButton)sender).DataContext;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl
                    + "comment/" + reply.ReplyInfo.RPL_CommentID;
                var comment = JObject.Parse(await client.GetStringAsync(new Uri(url)))
                    .ToObject<T_Comment>();
                var commentDto = await GetCommentDto(comment);
                url = HttpClientHelper.baseUrl
                    + "wallpaper/" + comment.C_WallpaperID;
                var wallpaper = JObject.Parse(await client.GetStringAsync(new Uri(url)))
                    .ToObject<T_Wallpaper>();
                rootFrame.Navigate(typeof(WallpaperMainPage), wallpaper);
                WallpaperMainPage.PageFrame.Navigate(typeof(ReplyPage), commentDto);//跳转到回复页面
                WallpaperMainPage.Page.HyperLinkButtonFocusChange("ReplyHyperlinkButton",
                    commentDto.PublisherInfo.U_Name + "的评论");
            }
        }

        /// <summary>
        /// 滑动到底部自动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ReplyScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await ReplyLVM.GetMessageReplysAsync(UserId, PageNum++, PageSize);
            }
        }

        /// <summary>
        /// 点击用户头像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplyUserButton_Click(object sender, RoutedEventArgs e)
        {
            var reply = (ReplyDto)((Button)sender).DataContext;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), reply.PublisherInfo);
        }

        /// <summary>
        /// 点击刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PageNum = 1;
            await ReplyLVM.GetMessageReplysAsync(UserId, PageNum++, PageSize);
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NotifyHelper.NotifyTypes.Contains((short)NotifyMessageType.回复))
            {
                NotifyHelper.NotifyTypes.Remove((short)NotifyMessageType.回复);
            }
            if (MessageMainPage.Page != null)
            {
                MessageMainPage.Page.HyperLinkButtonFocusChange("ReplyToUserHyperlinkButton");
            }
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
            await ReplyLVM.GetMessageReplysAsync(UserId, PageNum++, PageSize);
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 获取评论显示类实例
        /// </summary>
        /// <param name="comment">评论信息</param>
        /// <returns></returns>
        private async Task<CommentDto> GetCommentDto(T_Comment comment)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "user/" + comment.C_PublisherID;
                var response = await client.GetAsync(new Uri(url));
                var userInfoDto = JObject.Parse(await response.Content.ReadAsStringAsync())
                    .ToObject<UserInfoDto>();
                url = HttpClientHelper.baseUrl
                    + "download/picture/small/" + userInfoDto.U_Avatar;
                var image = await ImageHelper.GetImageAsync(client, url);
                return new CommentDto
                {
                    CommentInfo = comment,
                    PublisherInfo = userInfoDto,
                    PublisherAvatar = image,
                    DeleteButtonVisibility = UserId == comment.C_PublisherID ?
                        Visibility.Visible : Visibility.Collapsed,
                    AllReplyHyperlinkButtonDisplayText = comment.C_ReplyNum > 0 ?
                        "查看" + comment.C_ReplyNum + "条回复" : "回复"
                };
            }
        }
    }
}
