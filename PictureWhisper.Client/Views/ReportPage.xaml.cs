using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 举报页面
    /// </summary>
    public sealed partial class ReportPage : Page
    {
        private ReportReasonsListViewModel ReportReasonLVM { get; set; }
        private T_Report ReportInfo { get; set; }
        private string MainPageName { get; set; }
        private int UserId { get; set; }

        public ReportPage()
        {
            ReportReasonLVM = new ReportReasonsListViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击提交按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageTextBlock.Text = "错误信息：" + Environment.NewLine;
            //检查输入是否正确
            if (ReportTypeComboBox.SelectedValue != null)
            {
                ReportInfo.RPT_Reason = (short)ReportTypeComboBox.SelectedValue;
            }
            else
            {
                ErrorMessageTextBlock.Text += "· 未选择举报信息";
            }
            if (AddtionalTextBox.Text != string.Empty)
            {
                ReportInfo.RPT_Additional = AddtionalTextBox.Text;
            }
            else
            {
                ErrorMessageTextBlock.Text += "· 未输入补充信息";
            }
            if (ErrorMessageTextBlock.Text.Contains("·"))
            {
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "report";
                var content = new HttpStringContent(JObject.FromObject(ReportInfo).ToString());
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                var resp = await client.PostAsync(new Uri(url), content);//发送举报信息
                if (!resp.IsSuccessStatusCode)
                {
                    ErrorMessageTextBlock.Text += "· 发送失败" + Environment.NewLine;
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                    BackToMainPage();
                }
            }
        }

        /// <summary>
        /// 点击取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            BackToMainPage();
        }

        /// <summary>
        /// 跳转到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
                dynamic param = (dynamic)e.Parameter;
                ReportInfo = param.ReportInfo;
                MainPageName = param.MainPageName;
                await ReportReasonLVM.GetReportReasonsAsync();
            }
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            ReportTypeComboBox.SelectedIndex = 0;
            AddtionalTextBox.Text = string.Empty;
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 返回到主页面
        /// </summary>
        private void BackToMainPage()
        {
            switch (MainPageName)
            {
                case "WallpaperMainPage":
                    if (WallpaperMainPage.PageFrame.CanGoBack)
                    {
                        WallpaperMainPage.PageFrame.GoBack();
                    }
                    break;
                case "UserMainPage":
                    if (UserMainPage.PageFrame.CanGoBack)
                    {
                        UserMainPage.PageFrame.GoBack();
                    }
                    break;
                case "MessageMainPage":
                    if (MessageMainPage.PageFrame.CanGoBack)
                    {
                        MessageMainPage.PageFrame.GoBack();
                    }
                    break;
                default:
                    if (WallpaperMainPage.PageFrame.CanGoBack)
                    {
                        WallpaperMainPage.PageFrame.GoBack();
                    }
                    break;
            }
        }
    }
}
