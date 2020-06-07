using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 评论消息页面
    /// </summary>
    public sealed partial class MessageCommentPage : Page
    {
        private CommentListViewModel CommentLVM { get; set; }
        private int UserId { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }

        public MessageCommentPage()
        {
            CommentLVM = new CommentListViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击回复按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ReplyHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            var comment = (CommentDto)((HyperlinkButton)sender).DataContext;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl
                    + "wallpaper/" + comment.CommentInfo.C_WallpaperID;
                var wallpaper = JObject.Parse(await client.GetStringAsync(new Uri(url)))
                    .ToObject<T_Wallpaper>();
                rootFrame.Navigate(typeof(WallpaperMainPage), wallpaper);
                WallpaperMainPage.PageFrame.Navigate(typeof(ReplyPage), comment);//跳转到回复页面
                WallpaperMainPage.Page.HyperLinkButtonFocusChange("ReplyHyperlinkButton",
                    comment.PublisherInfo.U_Name + "的评论");
            }
        }

        /// <summary>
        /// 点击举报按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommentReportButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = (CommentDto)((MenuFlyoutItem)sender).DataContext;
            var reportInfo = new T_Report();
            reportInfo.RPT_ReporterID = UserId;
            reportInfo.RPT_ReportedID = comment.CommentInfo.C_ID;
            reportInfo.RPT_Type = (short)ReportType.评论;
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
        private async void CommentDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = (CommentDto)((MenuFlyoutItem)sender).DataContext;
            var contentDialog = new ContentDialog
            {
                Title = "删除评论",
                Content = "是否删除评论？",
                PrimaryButtonText = "是",
                SecondaryButtonText = "否"
            };
            contentDialog.PrimaryButtonClick += async (_sender, _e) =>
            {
                using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                {
                    var url = HttpClientHelper.baseUrl + "comment/" + comment.CommentInfo.C_ID;
                    var resp = await client.DeleteAsync(new Uri(url));
                    if (resp.IsSuccessStatusCode)
                    {
                        CommentLVM.WallpaperComments.Remove(comment);
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
        /// 滑动到底部自动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CommentScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await CommentLVM.GetMessageCommentsAsync(UserId, PageNum++, PageSize);
            }
        }

        /// <summary>
        /// 点击用户头像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = (CommentDto)((Button)sender).DataContext;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), comment.PublisherInfo);
        }

        /// <summary>
        /// 点击刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PageNum = 1;
            await CommentLVM.GetMessageCommentsAsync(UserId, PageNum++, PageSize);
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NotifyHelper.NotifyTypes.Contains((short)NotifyMessageType.评论))
            {
                NotifyHelper.NotifyTypes.Remove((short)NotifyMessageType.评论);
            }
            if (MessageMainPage.Page != null)
            {
                MessageMainPage.Page.HyperLinkButtonFocusChange("CommentToUserHyperlinkButton");
            }
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
            await CommentLVM.GetMessageCommentsAsync(UserId, PageNum++, PageSize);
            base.OnNavigatedTo(e);
        }
    }
}
