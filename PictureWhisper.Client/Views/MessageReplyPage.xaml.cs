using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helpers;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

        private void ReplyReportButton_Click(object sender, RoutedEventArgs e)
        {
            var reply = (ReplyDto)((Button)sender).DataContext;
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
                var commentDto = GetCommentDto(comment);
                url = HttpClientHelper.baseUrl
                    + "wallpaper/" + comment.C_WallpaperID;
                var wallpaper = JObject.Parse(await client.GetStringAsync(new Uri(url)))
                    .ToObject<T_Wallpaper>();
                rootFrame.Navigate(typeof(WallpaperMainPage), wallpaper);
                WallpaperMainPage.PageFrame.Navigate(typeof(ReplyPage), commentDto);
            }
        }

        private async void ReplyScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await ReplyLVM.GetMessageReplysAsync(UserId, PageNum++, PageSize);
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
            await ReplyLVM.GetMessageReplysAsync(UserId, PageNum++, PageSize);
            base.OnNavigatedTo(e);
        }

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
