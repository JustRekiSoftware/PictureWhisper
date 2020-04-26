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
    /// An empty page that can be used on its own or navigated to within a Frame.
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
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void AllReplyHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = (CommentDto)((HyperlinkButton)sender).DataContext;
            WallpaperMainPage.PageFrame.Navigate(typeof(ReplyPage), comment);
            WallpaperMainPage.Page.HyperLinkButtonFocusChange("ReplyHyperlinkButton", 
                "回复" + comment.PublisherInfo.U_Name);
        }

        private void CommentReportButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = (CommentDto)((Button)sender).DataContext;
            var reportInfo = new T_Report();
            reportInfo.RPT_ReporterID = UserId;
            reportInfo.RPT_ReportedID = comment.CommentInfo.C_ID;
            reportInfo.RPT_Type = (short)ReportType.评论;
            dynamic param = new
            {
                ReportInfo = reportInfo,
                MainPageName = "WallpaperMainPage"
            };
            WallpaperMainPage.PageFrame.Navigate(typeof(ReportPage), param);
        }

        private async void CommentDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = (CommentDto)((Button)sender).DataContext;
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

        private async void CommentScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
            }
        }

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
                var resp = await client.PostAsync(new Uri(url), content);
                if (!resp.IsSuccessStatusCode)
                {
                    ErrorMsgTextBlock.Text += "· 发送失败" + Environment.NewLine;
                    ErrorMsgTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    ErrorMsgTextBlock.Visibility = Visibility.Collapsed;
                    CommentTextBox.Text = string.Empty;
                    PageNum = 1;
                    await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
                }
            }
        }

        public void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = (CommentDto)((Button)sender).DataContext;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), comment.PublisherInfo);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            if (e.Parameter != null)
            {
                var wallpaper = (T_Wallpaper)e.Parameter;
                if (WallpaperInfo == null)
                {
                    WallpaperInfo = wallpaper;
                    PageNum = 1;
                    await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
                }
                else
                {
                    if (WallpaperInfo.W_ID != wallpaper.W_ID)
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
