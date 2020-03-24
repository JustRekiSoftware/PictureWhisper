using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helpers;
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

        private async void CommentScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await CommentLVM.GetMessageCommentsAsync(UserId, PageNum++, PageSize);
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
            await CommentLVM.GetWallpaperCommentsAsync(UserId, PageNum++, PageSize);
            base.OnNavigatedTo(e);
        }
    }
}
