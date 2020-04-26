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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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
                WallpaperMainPage.PageFrame.Navigate(typeof(ReplyPage), comment);
            }
        }

        private void CommentReportButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = (CommentDto)((Button)sender).DataContext;
            var reportInfo = new T_Report();
            reportInfo.RPT_ReporterID = UserId;
            reportInfo.RPT_ReportedID = comment.CommentInfo.C_ID;
            reportInfo.RPT_Type = (short)ReportType.评论;
            MainPage.PageFrame.Navigate(typeof(ReportPage), reportInfo);
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
                await CommentLVM.GetMessageCommentsAsync(UserId, PageNum++, PageSize);
            }
        }

        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = (CommentDto)((Button)sender).DataContext;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), comment.PublisherInfo);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
            await CommentLVM.GetMessageCommentsAsync(UserId, PageNum++, PageSize);
            base.OnNavigatedTo(e);
        }
    }
}
