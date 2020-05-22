using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 举报处理页面
    /// </summary>
    public sealed partial class ReportReviewPage : Page
    {
        private ReportListViewModel ReportLVM { get; set; }
        private int CurrentIndex { get; set; }
        private ReviewViewModel ReviewVM { get; set; }
        private readonly int Count = 10;
        private int UserId { get; set; }

        public ReportReviewPage()
        {
            ReportLVM = new ReportListViewModel();
            ReviewVM = new ReviewViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击审核通过按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PassButton_Click(object sender, RoutedEventArgs e)
        {
            await SendReviewInfoAsync(true);
        }

        /// <summary>
        /// 点击审核不通过按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NotPassButton_Click(object sender, RoutedEventArgs e)
        {
            await SendReviewInfoAsync(false);
        }

        /// <summary>
        /// 点击上一个或刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex == 0)//刷新
            {
                ReportLVM.Reports.Clear();
                await LoadReportsAsync();
            }
            if (CurrentIndex > 0)//上一个
            {
                PrevFontIcon.Glyph = "\xE76B";
                CurrentIndex--;
                ReviewVM.Image = ReportLVM.Reports[CurrentIndex].Image;
                ReviewVM.ReportDto = ReportLVM.Reports[CurrentIndex];
            }
            if (CurrentIndex == 0)//当前显示的举报信息为第一条时，将上一个按钮变为刷新按钮
            {
                PrevFontIcon.Glyph = "\xE72C";
            }
        }

        /// <summary>
        /// 点击下一个按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex < ReportLVM.Reports.Count - 1)
            {
                CurrentIndex++;
                ReviewVM.Image = ReportLVM.Reports[CurrentIndex].Image;
                ReviewVM.ReportDto = ReportLVM.Reports[CurrentIndex];
            }
            if (CurrentIndex > ReportLVM.Reports.Count - 5)//当要到列表末尾时自动加载
            {
                CurrentIndex++;
                ReviewVM.Image = ReportLVM.Reports[CurrentIndex].Image;
                ReviewVM.ReportDto = ReportLVM.Reports[CurrentIndex];
                await LoadReportsAsync();
            }
            if (CurrentIndex > 0)
            {
                PrevFontIcon.Glyph = "\xE70E";
            }
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            ReportLVM.Reports.Clear();
            CurrentIndex = 0;
            await LoadReportsAsync();
            if (ReportLVM.Reports.Count == 0)
            {
                PrevButton.Visibility = Visibility.Collapsed;
                NextButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ReviewVM.Image = ReportLVM.Reports[CurrentIndex].Image;
                ReviewVM.ReportDto = ReportLVM.Reports[CurrentIndex];
                PrevButton.Visibility = Visibility.Visible;
                NextButton.Visibility = Visibility.Visible;
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 加载举报信息
        /// </summary>
        /// <returns></returns>
        private async Task LoadReportsAsync()
        {
            if (ReportLVM.Reports.Count == 0)
            {
                await ReportLVM.GetReportsAsync(5);
            }
            else
            {
                await ReportLVM.GetReportsAsync(Count);
            }
            if (CurrentIndex < ReportLVM.Reports.Count)
            {
                ReviewVM.Image = ReportLVM.Reports[CurrentIndex].Image;
                ReviewVM.ReportDto = ReportLVM.Reports[CurrentIndex];
            }
        }

        /// <summary>
        /// 发送审核信息
        /// </summary>
        /// <param name="isPass"></param>
        /// <returns></returns>
        private async Task SendReviewInfoAsync(bool isPass)
        {
            if (ReportLVM.Reports.Count == 0)
            {
                ReviewVM.Image = new BitmapImage();
                ReviewVM.ReportDto = new ReportDto();
                return;
            }
            ErrorMessageTextBlock.Text += "错误信息：" + Environment.NewLine;
            var reviewInfo = new T_Review()
            {
                RV_ReviewerID = UserId,
                RV_ReviewedID = ReportLVM
                    .Reports[CurrentIndex].ReportInfo.RPT_ID,
                RV_Type = (short)ReviewType.举报审核,
                RV_Result = isPass,
                RV_MsgToReporterID = ReportLVM
                    .Reports[CurrentIndex].ReportInfo.RPT_ReporterID,
                RV_MsgToReportedID = ReportLVM
                    .Reports[CurrentIndex].MessageToId
            };
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "review";
                var content = new HttpStringContent(JObject.FromObject(reviewInfo).ToString());
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                var resp = await client.PostAsync(new Uri(url), content);//发送审核信息
                if (!resp.IsSuccessStatusCode)
                {
                    ErrorMessageTextBlock.Text += "· 发送失败" + Environment.NewLine;
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
                else//发送成功后从当前列表中删除该举报信息
                {
                    ReportLVM.Reports.Remove(ReportLVM
                        .Reports[CurrentIndex]);
                    if (ReportLVM.Reports.Count == 0)
                    {
                        CurrentIndex = 0;
                        ReviewVM.Image = new BitmapImage();
                        ReviewVM.ReportDto = new ReportDto();
                        return;
                    }
                    if (CurrentIndex < ReportLVM.Reports.Count - 1)
                    {
                        CurrentIndex++;
                        ReviewVM.Image = ReportLVM.Reports[CurrentIndex].Image;
                        ReviewVM.ReportDto = ReportLVM.Reports[CurrentIndex];
                    }
                    if (CurrentIndex > ReportLVM.Reports.Count - 5)//当要到列表末尾时自动加载
                    {
                        await LoadReportsAsync();
                    }
                    ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
