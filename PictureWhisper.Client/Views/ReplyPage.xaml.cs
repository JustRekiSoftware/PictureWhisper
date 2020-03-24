﻿using Newtonsoft.Json.Linq;
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
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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
        }

        private async void ReplyScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await ReplyLVM.GetCommentReplysAsync(CommentDto.CommentInfo.C_ID, PageNum++, PageSize);
            }
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
                MainPageName = "WallpaperMainPage"
            };
            WallpaperMainPage.PageFrame.Navigate(typeof(ReportPage), param);
        }

        private async void ReplyDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var reply = (ReplyDto)((Button)sender).DataContext;
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

        private void ReplyHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var reply = (ReplyDto)((Button)sender).DataContext;
            ReplyTo = reply;
            ReplyTextBox.PlaceholderText = "@" + ReplyTo.PublisherInfo.U_Name + ": ";
        }

        private async void ReplySendButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMsgTextBlock.Text += "错误信息：" + Environment.NewLine;
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
                if (!resp.IsSuccessStatusCode)
                {
                    ErrorMsgTextBlock.Text += "· 发送失败" + Environment.NewLine;
                    ErrorMsgTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    ErrorMsgTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

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

        private void ReplyCommentHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ReplyTo = null;
            ReplyTextBox.PlaceholderText = string.Empty;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
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
                UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            }
            base.OnNavigatedTo(e);
        }

        private void CommentUserButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReplyUserButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
