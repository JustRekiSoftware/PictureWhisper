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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReviewMainPage : Page
    {
        private T_SigninInfo SigninInfo { get; set; }
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

        private void WallpaperReviewHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperReviewPage), SigninInfo);
            HyperLinkButtonFocusChange("WallpaperReviewHyperlinkButton");
        }

        private void ReportReviewHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(ReportReviewPage), SigninInfo);
            HyperLinkButtonFocusChange("ReportReviewHyperlinkButton");
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            SigninInfo = SQLiteHelper.GetSigninInfo();
            if (SigninInfo.SI_Type == (short)UserType.审核人员)
            {
                WallpaperReviewHyperlinkButton.Visibility = Visibility.Visible;
                ReportReviewHyperlinkButton.Visibility = Visibility.Collapsed;
                ContentFrame.Navigate(typeof(WallpaperReviewPage), SigninInfo);
                HyperLinkButtonFocusChange("WallpaperReviewHyperlinkButton");
            }
            else if (SigninInfo.SI_Type == (short)UserType.举报处理人员)
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
                ImageVM.Image = await ImageHelper.GetImageAsync(client, url);
            }
            base.OnNavigatedTo(e);
        }

        public void HyperLinkButtonFocusChange(string currentFocusName, string content = null)
        {
            HyperlinkButton currentFocus;
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
            if (currentFocus.Visibility == Visibility.Collapsed)
            {
                currentFocus.Visibility = Visibility.Visible;
                if (content != null)
                {
                    currentFocus.Content = content;
                }
            }
            if (LastFocus != null)
            {
                LastFocus.Foreground = new SolidColorBrush(ColorHelper.GetGray());
            }
            LastFocus = currentFocus;
            LastFocus.Foreground = new SolidColorBrush(ColorHelper.GetAccentColor());
        }

        private async void SignoutButton_Click(object sender, RoutedEventArgs e)
        {
            await SQLiteHelper.RemoveSigninInfoAsync(SQLiteHelper.GetSigninInfo());
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(SigninPage));
        }

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
                var wallpaper = JObject.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = wallpaper.ToObject<UserInfoDto>();
                if (result == null)
                {
                    return;
                }
                var rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(UserMainPage), result);
            }
        }
    }
}
