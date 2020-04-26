using PictureWhisper.Client.Domain.Concrete;
using PictureWhisper.Client.Helper;
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
using Windows.Web.Http;
using System.Drawing;
using Windows.UI.Xaml.Media.Imaging;
using PictureWhisper.Client.Views;
using Windows.UI.ViewManagement;
using Windows.Graphics.Imaging;
using PictureWhisper.Domain.Entites;
using PictureWhisper.Client.Domain.Entities;
using Newtonsoft.Json.Linq;
using PictureWhisper.Client.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PictureWhisper.Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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
            NavigationCacheMode = NavigationCacheMode.Enabled;
            PageFrame = ContentFrame;
            Page = this;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var signinInfo = SQLiteHelper.GetSigninInfo();
            UserId = signinInfo.SI_UserID;
            await GetUserInfoAsync(signinInfo);
            ContentFrame.Navigate(typeof(WallpaperRecommendPage));
            HyperLinkButtonFocusChange("WallpaperRecommendHyperlinkButton");
        }

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
                            .ToObject<UserInfoDto>();
                    }
                    url = HttpClientHelper.baseUrl + "download/picture/small/" + signinInfo.SI_Avatar;
                    ImageVM.Image = await ImageHelper.GetImageAsync(client, url);
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(SearchPage), "wallpaper");
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), UserInfo);
        }

        private void WallpaperRecommendHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperRecommendPage));
            HyperLinkButtonFocusChange("WallpaperRecommendHyperlinkButton");
        }

        private void WallpaperTypeHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperTypePage));
            HyperLinkButtonFocusChange("WallpaperTypeHyperlinkButton");
        }

        private void SpaceHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(SpacePage), UserInfo);
            HyperLinkButtonFocusChange("SpaceHyperlinkButton");
        }

        private void WallpaperSearchResultHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperSearchResultPage));
            HyperLinkButtonFocusChange("WallpaperSearchResultHyperlinkButton");
        }

        private void UserSearchResultHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(UserSearchResultPage));
            HyperLinkButtonFocusChange("UserSearchResultHyperlinkButton");
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(SettingPage));
        }

        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MessageMainPage), UserId);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperPublishPage), UserId);
        }

        public void HyperLinkButtonFocusChange(string currentFocusName, string content = null)
        {
            HyperlinkButton currentFocus;
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
            if (currentFocus.Visibility == Visibility.Collapsed)
            {
                currentFocus.Visibility = Visibility.Visible;
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
            }
            if (LastFocus != null)
            {
                LastFocus.Foreground = new SolidColorBrush(ColorHelper.GetGray());
            }
            LastFocus = currentFocus;
            LastFocus.Foreground = new SolidColorBrush(ColorHelper.GetAccentColor());
        }
    }
}
