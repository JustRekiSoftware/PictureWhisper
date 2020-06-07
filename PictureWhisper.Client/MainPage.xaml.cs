using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Client.Views;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PictureWhisper.Client
{
    /// <summary>
    /// 主页面
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private UserInfoDto UserInfo { get; set; }
        private int UserId { get; set; }
        private ImageViewModel ImageVM { get; set; }
        private HyperlinkButton LastFocus { get; set; }

        public static Frame PageFrame { get; private set; }
        public static MainPage Page { get; private set; }

        public MainPage()
        {
            ImageVM = new ImageViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;//启用缓存
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
            await GetUserInfoAsync(signinInfo);
            ContentFrame.Navigate(typeof(WallpaperRecommendPage));//自动导航到壁纸推荐页面
            HyperLinkButtonFocusChange("WallpaperRecommendHyperlinkButton");//高亮壁纸推荐按钮
            NotifyHelper.ConfigConnect();//配置消息提示服务器连接
            NotifyHelper.ConnectionOn<short>("NotifyNewMessage", (type) =>
            {
                if (!NotifyHelper.NotifyTypes.Contains(type))
                {
                    NotifyHelper.NotifyTypes.Add(type);
                }
                MessageButton.Foreground = new SolidColorBrush(ColorHelper.GetLighterAccentColor());
            });//配置客户端函数
            NotifyHelper.ConnectionOn<List<short>>("NotifyNewMessages", (types) =>
            {
                MessageButton.Foreground = new SolidColorBrush(ColorHelper.GetLighterAccentColor());
            });//配置客户端函数
            await NotifyHelper.StartAsync();//连接服务器
            await NotifyHelper.SignInAsync();//向服务器发送注册请求
            await NotifyHelper.CheckNewMessageAsync();//向服务器发送检查新消息请求
            if (NotifyHelper.NotifyTypes.Count > 0)
            {
                MessageButton.Foreground = new SolidColorBrush(ColorHelper.GetLighterAccentColor());
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="signinInfo">登录信息</param>
        /// <returns></returns>
        private async Task GetUserInfoAsync(T_SigninInfo signinInfo)
        {
            if (signinInfo != null && signinInfo.SI_Avatar != string.Empty)
            {
                using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                {
                    var url = HttpClientHelper.baseUrl + "user/" + signinInfo.SI_UserID;
                    var resp = await client.GetAsync(new Uri(url));
                    if (resp.IsSuccessStatusCode)
                    {
                        UserInfo = JObject.Parse(await resp.Content.ReadAsStringAsync())
                            .ToObject<UserInfoDto>();//获取用户信息
                    }
                    url = HttpClientHelper.baseUrl + "download/picture/small/" + signinInfo.SI_Avatar;
                    ImageVM.Image = await ImageHelper.GetImageAsync(client, url);//获取头像
                }
            }
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
        /// 点击搜索按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(SearchPage), "wallpaper");
        }

        /// <summary>
        /// 点击用户头像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), UserInfo);//UserMainPage使用rootFrame跳转
        }

        /// <summary>
        /// 点击壁纸推荐超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperRecommendHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperRecommendPage));
            HyperLinkButtonFocusChange("WallpaperRecommendHyperlinkButton");
        }

        /// <summary>
        /// 点击壁纸分区超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperTypeHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperTypePage));
            HyperLinkButtonFocusChange("WallpaperTypeHyperlinkButton");
        }

        /// <summary>
        /// 点击动态超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpaceHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(SpacePage), UserInfo);
            HyperLinkButtonFocusChange("SpaceHyperlinkButton");
        }

        /// <summary>
        /// 点击壁纸搜索超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperSearchResultHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperSearchResultPage));
            HyperLinkButtonFocusChange("WallpaperSearchResultHyperlinkButton");
        }

        /// <summary>
        /// 点击用户搜索超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserSearchResultHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(UserSearchResultPage));
            HyperLinkButtonFocusChange("UserSearchResultHyperlinkButton");
        }

        /// <summary>
        /// 点击设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(SettingPage));
        }

        /// <summary>
        /// 点击消息按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MessageMainPage), UserId);
        }

        /// <summary>
        /// 点击壁纸发布按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperPublishPage), UserId);
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
                case "WallpaperRecommendHyperlinkButton":
                    currentFocus = WallpaperRecommendHyperlinkButton;
                    break;
                case "WallpaperTypeHyperlinkButton":
                    currentFocus = WallpaperTypeHyperlinkButton;
                    break;
                case "SpaceHyperlinkButton":
                    currentFocus = SpaceHyperlinkButton;
                    break;
                case "WallpaperSearchResultHyperlinkButton":
                    currentFocus = WallpaperSearchResultHyperlinkButton;
                    break;
                case "UserSearchResultHyperlinkButton":
                    currentFocus = UserSearchResultHyperlinkButton;
                    break;
                default:
                    currentFocus = WallpaperRecommendHyperlinkButton;
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
                switch (currentFocusName)
                {
                    case "WallpaperSearchResultHyperlinkButton":
                        WallpaperSearchResultHyperlinkButtonTextBlock.Text = content;
                        break;
                    case "UserSearchResultHyperlinkButton":
                        UserSearchResultHyperlinkButtonTextBlock.Text = content;
                        break;
                    default:
                        break;
                }
            }
            //将上一次高亮超链接按钮的颜色更改为默认色
            if (LastFocus != null)
            {
                LastFocus.Foreground = new SolidColorBrush(ColorHelper.GetForegroudColor());
            }
            //将当前高亮超链接按钮保存为上一次高亮超链接按钮，并更改颜色为高亮色
            LastFocus = currentFocus;
            LastFocus.Foreground = new SolidColorBrush(ColorHelper.GetAccentColor());
        }
    }
}
