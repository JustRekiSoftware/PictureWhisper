using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Client.Views;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace PictureWhisper.Client
{
    /// <summary>
    /// 管理员主页面
    /// </summary>
    public sealed partial class AdminMainPage : Page
    {
        private int UserId { get; set; }
        private HyperlinkButton LastFocus { get; set; }
        public T_SigninInfo SigninInfo { get; set; }
        public ImageViewModel ImageVM { get; set; }

        public static AdminMainPage Page { get; private set; }
        public static Frame PageFrame { get; private set; }

        public AdminMainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;//启用缓存
            ImageVM = new ImageViewModel();
            PageFrame = ContentFrame;
            Page = this;
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
        /// 点击广告壁纸上传超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdWallpaperUploadHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic param = new
            {
                UserId = SigninInfo.SI_UserID,
                Todo = "AdWallpaperUpload"
            };
            ContentFrame.Navigate(typeof(WallpaperPublishPage), param);
            HyperLinkButtonFocusChange("AdWallpaperUploadHyperlinkButton");
        }

        /// <summary>
        /// 点击每日壁纸上传超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TodayWallpaperUploadHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(TodayWallpaperUploadPage), UserId);
            HyperLinkButtonFocusChange("TodayWallpaperUploadHyperlinkButton");
        }

        /// <summary>
        /// 点击默认头像上传超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefaultAvatarUploadHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(DefaultAvatarUploadPage), SigninInfo);
            HyperLinkButtonFocusChange("DefaultAvatarUploadHyperlinkButton");
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
                case "AdWallpaperUploadHyperlinkButton":
                    currentFocus = AdWallpaperUploadHyperlinkButton;
                    break;
                case "TodayWallpaperUploadHyperlinkButton":
                    currentFocus = TodayWallpaperUploadHyperlinkButton;
                    break;
                case "DefaultAvatarUploadHyperlinkButton":
                    currentFocus = DefaultAvatarUploadHyperlinkButton;
                    break;
                default:
                    currentFocus = AdWallpaperUploadHyperlinkButton;
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
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            SigninInfo = SQLiteHelper.GetSigninInfo();
            UserId = SigninInfo.SI_UserID;
            dynamic param = new
            {
                UserId = SigninInfo.SI_UserID,
                Todo = "AdWallpaperUpload"
            };
            ContentFrame.Navigate(typeof(WallpaperPublishPage), param);
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl +
                    "download/picture/origin/" + SigninInfo.SI_Avatar;
                ImageVM.Image = await ImageHelper.GetImageAsync(client, url);//获取头像
            }
            base.OnNavigatedTo(e);
        }
    }
}
