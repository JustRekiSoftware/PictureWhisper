using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 评论页面
    /// </summary>
    public sealed partial class CommentPage : Page
    {
        private CommentListViewModel CommentLVM { get; set; }
        private T_Wallpaper WallpaperInfo { get; set; }
        private int UserId { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }

        public CommentPage()
        {
            CommentLVM = new CommentListViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;//启用缓存
        }

        /// <summary>
        /// 点击回复按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllReplyHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = (CommentDto)((HyperlinkButton)sender).DataContext;
            WallpaperMainPage.PageFrame.Navigate(typeof(ReplyPage), comment);//导航到回复页面
            WallpaperMainPage.Page.HyperLinkButtonFocusChange("ReplyHyperlinkButton", 
                comment.PublisherInfo.U_Name + "的评论");
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
                MainPageName = "WallpaperMainPage"
            };
            WallpaperMainPage.PageFrame.Navigate(typeof(ReportPage), param);//跳转到举报页面
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
                await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
            }
        }

        /// <summary>
        /// 点击发表评论按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CommentSendButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMsgTextBlock.Text += "错误信息：" + Environment.NewLine;
            if (CommentTextBox.Text == string.Empty)
            {
                ErrorMsgTextBlock.Text += "· 未输入评论" + Environment.NewLine;
                ErrorMsgTextBlock.Visibility = Visibility.Visible;
                return;
            }
            var comment = new T_Comment
            {
                C_PublisherID = UserId,
                C_Content = CommentTextBox.Text,
                C_ReceiverID = WallpaperInfo.W_PublisherID,
                C_WallpaperID = WallpaperInfo.W_ID
            };
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "comment";
                var content = new HttpStringContent(JObject.FromObject(comment).ToString());
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                var resp = await client.PostAsync(new Uri(url), content);//发表评论
                if (!resp.IsSuccessStatusCode)
                {
                    ErrorMsgTextBlock.Text += "· 发送失败" + Environment.NewLine;
                    ErrorMsgTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    ErrorMsgTextBlock.Visibility = Visibility.Collapsed;
                    CommentTextBox.Text = string.Empty;
                    //PageNum = 1;
                    //await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
                }
            }
        }

        /// <summary>
        /// 点击用户头像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AvatarButton_Click(object sender, RoutedEventArgs e)
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
            await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (WallpaperMainPage.Page != null)
            {
                WallpaperMainPage.Page.HyperLinkButtonFocusChange("CommentHyperlinkButton");
            }
            ErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            if (e.Parameter != null)
            {
                var wallpaper = (T_Wallpaper)e.Parameter;
                if (WallpaperInfo == null)//第一次进入
                {
                    WallpaperInfo = wallpaper;
                    PageNum = 1;
                    await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
                }
                else
                {
                    if (WallpaperInfo.W_ID != wallpaper.W_ID)//不同的壁纸评论
                    {
                        WallpaperInfo = wallpaper;
                        PageNum = 1;
                        await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
                    }
                }
                UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            }
            base.OnNavigatedTo(e);
        }
    }
}
