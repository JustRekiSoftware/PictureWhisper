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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportReviewPage : Page
    {
        private ReportListViewModel ReportLVM { get; set; }
        private int CurrentIndex { get; set; }
        private BitmapImage CurrentImage { get; set; }
        private ReportDto CurrentReport { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private int UserId { get; set; }

        public ReportReviewPage()
        {
            ReportLVM = new ReportListViewModel();
            this.InitializeComponent();
        }

        private async void PassButton_Click(object sender, RoutedEventArgs e)
        {
            await SendReviewInfoAsync(true);
        }

        private async void NotPassButton_Click(object sender, RoutedEventArgs e)
        {
            await SendReviewInfoAsync(false);
        }

        private async void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex == 0)
            {
                PageNum = 1;
                await LoadReportsAsync(PageNum++);
            }
            if (CurrentIndex > 0)
            {
                PrevFontIcon.Glyph = "&#xE70E;";
                CurrentIndex--;
                CurrentImage = ReportLVM.Reports[CurrentIndex].Image;
                CurrentReport = ReportLVM.Reports[CurrentIndex];
            }
            if (CurrentIndex == 0)
            {
                PrevFontIcon.Glyph = "&#xE72C;";
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex < ReportLVM.Reports.Count)
            {
                CurrentIndex++;
                CurrentImage = ReportLVM.Reports[CurrentIndex].Image;
                CurrentReport = ReportLVM.Reports[CurrentIndex];
            }
            if (CurrentIndex > ReportLVM.Reports.Count - 5)
            {
                await LoadReportsAsync(PageNum++);
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
            CurrentIndex = 0;
            await LoadReportsAsync(PageNum++);
            base.OnNavigatedTo(e);
        }

        private async Task LoadReportsAsync(int page)
        {
            await ReportLVM.GetReportsAsync(page, PageSize);
        }

        private async Task SendReviewInfoAsync(bool isPass)
        {
            ErrorMessageTextBlock.Text += "错误信息：" + Environment.NewLine;
            var reviewInfo = new T_Review()
            {
                RV_ReviewerID = UserId,
                RV_ReviewedID = ReportLVM
                    .Reports[CurrentIndex].ReportInfo.RPT_ID,
                RV_Type = (short)ReviewType.举报审核,
                RV_Result = isPass,
                RV_MsgToReporterID = 0,
                RV_MsgToReportedID = ReportLVM
                    .Reports[CurrentIndex].MessageToId
            };
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "review";
                var content = new HttpStringContent(JObject.FromObject(reviewInfo).ToString());
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                var resp = await client.PostAsync(new Uri(url), content);
                if (!resp.IsSuccessStatusCode)
                {
                    ErrorMessageTextBlock.Text += "· 发送失败" + Environment.NewLine;
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    ReportLVM.Reports.Remove(ReportLVM
                        .Reports[CurrentIndex]);
                    if (CurrentIndex < ReportLVM.Reports.Count)
                    {
                        CurrentIndex++;
                        CurrentImage = ReportLVM.Reports[CurrentIndex].Image;
                        CurrentReport = ReportLVM.Reports[CurrentIndex];
                    }
                    if (CurrentIndex > ReportLVM.Reports.Count - 5)
                    {
                        await LoadReportsAsync(PageNum++);
                    }
                    ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
