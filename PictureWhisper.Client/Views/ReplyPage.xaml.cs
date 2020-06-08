using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 回复页面
    /// </summary>
    public sealed partial class ReplyPage : Page
    {
        private ReplyListViewModel ReplyLVM { get; set; }
        private CommentDto CommentDto { get; set; }
        private int UserId { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private ReplyDto ReplyTo { get; set; }

        public ReplyPage()
        {
            ReplyLVM = new ReplyListViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ReplyTextBox.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
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
                await ReplyLVM.GetCommentReplysAsync(CommentDto.CommentInfo.C_ID, PageNum++, PageSize);
            }
        }

        /// <summary>
        /// 点击回复的举报按钮
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
                MainPageName = "WallpaperMainPage"
            };
            WallpaperMainPage.PageFrame.Navigate(typeof(ReportPage), param);
        }

        /// <summary>
        /// 点击回复的删除按钮
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
        /// 点击回复的回复超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplyHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var reply = (ReplyDto)((HyperlinkButton)sender).DataContext;
            ReplyTo = reply;
            ReplyTextBox.PlaceholderText = "@" + ReplyTo.PublisherInfo.U_Name + ": ";
        }

        /// <summary>
        /// 点击发表回复按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ReplySendButton_Click(object sender, RoutedEventArgs e)
        {
            await ReplyNow();
        }

        /// <summary>
        /// 点击评论的举报按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommentReportButton_Click(object sender, RoutedEventArgs e)
        {
            var reportInfo = new T_Report();
            reportInfo.RPT_ReporterID = UserId;
            reportInfo.RPT_ReportedID = CommentDto.CommentInfo.C_ID;
            reportInfo.RPT_Type = (short)ReportType.评论;
            dynamic param = new
            {
                ReportInfo = reportInfo,
                MainPageName = "WallpaperMainPage"
            };
            WallpaperMainPage.PageFrame.Navigate(typeof(ReportPage), param);
        }

        /// <summary>
        /// 点击评论的删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CommentDeleteButton_Click(object sender, RoutedEventArgs e)
        {
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
                    var url = HttpClientHelper.baseUrl + "comment/" + CommentDto.CommentInfo.C_ID;
                    var resp = await client.DeleteAsync(new Uri(url));
                    if (!resp.IsSuccessStatusCode)
                    {
                        if (WallpaperMainPage.PageFrame.CanGoBack)
                        {
                            WallpaperMainPage.PageFrame.GoBack();
                        }
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
        /// 点击评论的回复超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplyCommentHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ReplyTo = null;
            ReplyTextBox.PlaceholderText = "有什么想说的？";
        }

        /// <summary>
        /// 点击刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PageNum = 1;
            await ReplyLVM.GetCommentReplysAsync(CommentDto.CommentInfo.C_ID, PageNum++, PageSize);
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (WallpaperMainPage.Page != null)
            {
                WallpaperMainPage.Page.HyperLinkButtonFocusChange("ReplyHyperlinkButton");
            }
            ErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            if (e.Parameter != null)
            {
                var commentDto = (CommentDto)e.Parameter;
                if (commentDto != null)
                {
                    CommentDto = commentDto;
                    PageNum = 1;
                    await ReplyLVM.GetCommentReplysAsync(CommentDto.CommentInfo.C_ID, PageNum++, PageSize);
                }
            }
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 点击评论的用户头像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommentUserButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), CommentDto.PublisherInfo);
        }

        /// <summary>
        /// 点击回复的用户头像
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
        /// 拦截回复输入框Control + Enter事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Dispatcher_AcceleratorKeyActivated(object sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                {
                    switch (args.VirtualKey)
                    {
                        case VirtualKey.Enter:
                            await ReplyNow();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 执行回复
        /// </summary>
        /// <returns></returns>
        private async Task ReplyNow()
        {
            ErrorMsgTextBlock.Text += "错误信息：" + Environment.NewLine;
            //检查输入是否正确
            if (ReplyTextBox.Text == string.Empty)
            {
                ErrorMsgTextBlock.Text += "· 未输入评论" + Environment.NewLine;
                ErrorMsgTextBlock.Visibility = Visibility.Visible;
                return;
            }
            var reply = new T_Reply
            {
                RPL_PublisherID = UserId,
                RPL_CommentID = CommentDto.CommentInfo.C_ID,
                RPL_Content = ReplyTo == null ?
                    ReplyTextBox.Text : "@" + ReplyTo.PublisherInfo.U_Name + ": " + ReplyTextBox.Text,
                RPL_ReceiverID = ReplyTo == null ?
                    CommentDto.CommentInfo.C_PublisherID : ReplyTo.PublisherInfo.U_ID
            };
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "reply";
                var content = new HttpStringContent(JObject.FromObject(reply).ToString());
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                var resp = await client.PostAsync(new Uri(url), content);
                if (!resp.IsSuccessStatusCode)//发送失败显示错误提示
                {
                    ErrorMsgTextBlock.Text += "· 发送失败" + Environment.NewLine;
                    ErrorMsgTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    ErrorMsgTextBlock.Visibility = Visibility.Collapsed;
                    ReplyTextBox.Text = string.Empty;
                    //当现有评论内容为空或不能滚动时，刷新评论列表
                    if (ReplyScrollViewer.ExtentHeight == 0
                        || ReplyScrollViewer.ExtentHeight == ReplyScrollViewer.ViewportHeight)
                    {
                        PageNum = 1;
                        await ReplyLVM.GetCommentReplysAsync(CommentDto.CommentInfo.C_ID, PageNum++, PageSize);
                    }
                }
            }
        }
    }
}
