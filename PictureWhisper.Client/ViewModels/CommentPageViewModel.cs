using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Client.ViewModels
{
    public class CommentPageViewModel : BindableBase
    {
        //private CommentListViewModel CommentLVM { get; set; }
        //private T_Wallpaper WallpaperInfo { get; set; }
        //private int UserId { get; set; }
        //private readonly int PageSize = 20;
        //private int PageNum { get; set; }


        ///// <summary>
        ///// 点击回复按钮
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void AllReplyHyperlinkButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var comment = (CommentDto)((HyperlinkButton)sender).DataContext;
        //    WallpaperMainPage.PageFrame.Navigate(typeof(ReplyPage), comment);//导航到回复页面
        //    WallpaperMainPage.Page.HyperLinkButtonFocusChange("ReplyHyperlinkButton",
        //        comment.PublisherInfo.U_Name + "的评论");
        //}

        ///// <summary>
        ///// 点击举报按钮
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void CommentReportButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var comment = (CommentDto)((MenuFlyoutItem)sender).DataContext;
        //    var reportInfo = new T_Report();
        //    reportInfo.RPT_ReporterID = UserId;
        //    reportInfo.RPT_ReportedID = comment.CommentInfo.C_ID;
        //    reportInfo.RPT_Type = (short)ReportType.评论;
        //    dynamic param = new
        //    {
        //        ReportInfo = reportInfo,
        //        MainPageName = "WallpaperMainPage"
        //    };
        //    WallpaperMainPage.PageFrame.Navigate(typeof(ReportPage), param);//跳转到举报页面
        //}

        ///// <summary>
        ///// 点击删除按钮
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private async void CommentDeleteButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var comment = (CommentDto)((MenuFlyoutItem)sender).DataContext;
        //    var contentDialog = new ContentDialog
        //    {
        //        Title = "删除评论",
        //        Content = "是否删除评论？",
        //        PrimaryButtonText = "是",
        //        SecondaryButtonText = "否"
        //    };
        //    contentDialog.PrimaryButtonClick += async (_sender, _e) =>
        //    {
        //        using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
        //        {
        //            var url = HttpClientHelper.baseUrl + "comment/" + comment.CommentInfo.C_ID;
        //            var resp = await client.DeleteAsync(new Uri(url));
        //            if (resp.IsSuccessStatusCode)
        //            {
        //                CommentLVM.WallpaperComments.Remove(comment);
        //            }
        //            else
        //            {
        //                contentDialog.Hide();
        //            }
        //        }
        //    };
        //    contentDialog.SecondaryButtonClick += (_sender, _e) =>
        //    {
        //        contentDialog.Hide();
        //    };
        //    await contentDialog.ShowAsync();
        //}

        ///// <summary>
        ///// 滑动到底部自动加载
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private async void CommentScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        //{
        //    var scrollViewer = (ScrollViewer)sender;
        //    if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
        //    {
        //        await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
        //    }
        //}

        ///// <summary>
        ///// 点击发表评论按钮
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private async void CommentSendButton_Click(object sender, RoutedEventArgs e)
        //{
        //    await CommentNow();
        //}

        ///// <summary>
        ///// 点击用户头像
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void AvatarButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var comment = (CommentDto)((Button)sender).DataContext;
        //    var rootFrame = Window.Current.Content as Frame;
        //    rootFrame.Navigate(typeof(UserMainPage), comment.PublisherInfo);
        //}

        ///// <summary>
        ///// 点击刷新按钮
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        //{
        //    PageNum = 1;
        //    await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
        //}

        ///// <summary>
        ///// 拦截评论输入框回车事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private async void CommentTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        //{
        //    await CommentNow();
        //}

        ///// <summary>
        ///// 执行评论
        ///// </summary>
        ///// <returns></returns>
        //private async Task CommentNow()
        //{
        //    ErrorMsgTextBlock.Text += "错误信息：" + Environment.NewLine;
        //    if (CommentTextBox.Text == string.Empty)
        //    {
        //        ErrorMsgTextBlock.Text += "· 未输入评论" + Environment.NewLine;
        //        ErrorMsgTextBlock.Visibility = Visibility.Visible;
        //        return;
        //    }
        //    var comment = new T_Comment
        //    {
        //        C_PublisherID = UserId,
        //        C_Content = CommentTextBox.Text,
        //        C_ReceiverID = WallpaperInfo.W_PublisherID,
        //        C_WallpaperID = WallpaperInfo.W_ID
        //    };
        //    using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
        //    {
        //        var url = HttpClientHelper.baseUrl + "comment";
        //        var content = new HttpStringContent(JObject.FromObject(comment).ToString());
        //        content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
        //        var resp = await client.PostAsync(new Uri(url), content);//发表评论
        //        if (!resp.IsSuccessStatusCode)
        //        {
        //            ErrorMsgTextBlock.Text += "· 发送失败" + Environment.NewLine;
        //            ErrorMsgTextBlock.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            ErrorMsgTextBlock.Visibility = Visibility.Collapsed;
        //            CommentTextBox.Text = string.Empty;
        //            //当现有评论内容为空或不能滚动时，刷新评论列表
        //            if (CommentScrollViewer.ExtentHeight == 0
        //                || CommentScrollViewer.ExtentHeight == CommentScrollViewer.ViewportHeight)
        //            {
        //                PageNum = 1;
        //                await CommentLVM.GetWallpaperCommentsAsync(WallpaperInfo.W_ID, PageNum++, PageSize);
        //            }
        //        }
        //    }
        //}
    }
}
