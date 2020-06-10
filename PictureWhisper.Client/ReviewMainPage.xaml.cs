using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Client.Views;
using PictureWhisper.Domain.Entites;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client
{
    /// <summary>
    /// 审核主页面
    /// </summary>
    public sealed partial class ReviewMainPage : Page
    {
        private T_SigninInfo SigninInfo { get; set; }
        private int UserId { get; set; }
        private ImageViewModel ImageVM { get; set; }
        private HyperlinkButton LastFocus { get; set; }

        public static ReviewMainPage Page { get; set; }
        public static Frame PageFrame { get; private set; }

        public ReviewMainPage()
        {
            this.InitializeComponent();
            ImageVM = new ImageViewModel();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            PageFrame = ContentFrame;
            Page = this;
        }

        /// <summary>
        /// 页面加载后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var signinInfo = SQLiteHelper.GetSigninInfo();
            UserId = signinInfo.SI_UserID;
            ReviewHelper.ConfigConnect();//配置审核处理服务器连接
            await ReviewHelper.StartAsync();//连接服务器
            await ReviewHelper.SignInAsync();//向服务器发送注册请求
        }

        /// <summary>
        /// 点击返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }

        /// <summary>
        /// 点击壁纸审核超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperReviewHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperReviewPage), SigninInfo);
            HyperLinkButtonFocusChange("WallpaperReviewHyperlinkButton");
        }

        /// <summary>
        /// 点击举报处理超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportReviewHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(ReportReviewPage), SigninInfo);
            HyperLinkButtonFocusChange("ReportReviewHyperlinkButton");
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            SigninInfo = SQLiteHelper.GetSigninInfo();
            if (SigninInfo.SI_Type == (short)UserType.审核人员)//审核人员自动导航到壁纸审核页面
            {
                WallpaperReviewHyperlinkButton.Visibility = Visibility.Visible;
                ReportReviewHyperlinkButton.Visibility = Visibility.Collapsed;
                ContentFrame.Navigate(typeof(WallpaperReviewPage), SigninInfo);
                HyperLinkButtonFocusChange("WallpaperReviewHyperlinkButton");
            }
            else if (SigninInfo.SI_Type == (short)UserType.举报处理人员)//举报处理人员自动导航到举报处理页面
            {
                WallpaperReviewHyperlinkButton.Visibility = Visibility.Collapsed;
                ReportReviewHyperlinkButton.Visibility = Visibility.Visible;
                ContentFrame.Navigate(typeof(ReportReviewPage), SigninInfo);
                HyperLinkButtonFocusChange("ReportReviewHyperlinkButton");
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl +
                    "download/picture/origin/" + SigninInfo.SI_Avatar;
                ImageVM.Image = await ImageHelper.GetImageAsync(client, url);//获取头像
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 超链接按钮焦点改变事件
        /// </summary>
        /// <param name="currentFocusName">当前高亮的超链接按钮名</param>
        /// <param name="content">超链接按钮显示的内容，默认为null</param>
        public void HyperLinkButtonFocusChange(string currentFocusName, string content = null)
        {
            HyperlinkButton currentFocus;
            //获取当前高亮的超链接按钮
            switch (currentFocusName)
            {
                case "WallpaperReviewHyperlinkButton":
                    currentFocus = WallpaperReviewHyperlinkButton;
                    break;
                case "ReportReviewHyperlinkButton":
                    currentFocus = ReportReviewHyperlinkButton;
                    break;
                default:
                    currentFocus = WallpaperReviewHyperlinkButton;
                    break;
            }
            //显示当前高亮超链接按钮
            if (currentFocus.Visibility == Visibility.Collapsed)
            {
                currentFocus.Visibility = Visibility.Visible;
            }
            //更改超链接按钮内容
            if (content != null)
            {
                currentFocus.Content = content;
            }
            //将上一次高亮超链接按钮的颜色更改为默认色
            if (LastFocus != null)
            {
                LastFocus.Foreground = new SolidColorBrush(ColorHelper.GetHyperLinkButtonForegroundColor());
            }
            //将当前高亮超链接按钮保存为上一次高亮超链接按钮，并更改颜色为高亮色
            LastFocus = currentFocus;
            LastFocus.Foreground = new SolidColorBrush(ColorHelper.GetAccentColor());
        }

        /// <summary>
        /// 点击用户头像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UserButton_Click(object sender, RoutedEventArgs e)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "user/" + SigninInfo.SI_UserID;
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var userInfoDto = JObject.Parse(
                    await response.Content.ReadAsStringAsync()).ToObject<UserInfoDto>();
                if (userInfoDto == null)
                {
                    return;
                }
                var rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(UserMainPage), userInfoDto);
            }
        }
    }
}
