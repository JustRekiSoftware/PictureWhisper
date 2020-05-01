using PictureWhisper.Client.Helper;
using PictureWhisper.Client.Views;
using PictureWhisper.Domain.Entites;
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
    public sealed partial class UserMainPage : Page
    {
        private UserInfoDto UserInfo { get; set; }
        private int UserId { get; set; }
        private HyperlinkButton LastFocus { get; set; }

        public static UserMainPage Page { get; set; }
        public static Frame PageFrame { get; private set; }

        public UserMainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            PageFrame = ContentFrame;
            Page = this;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void UserInfoHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(UserPage), UserInfo);
            HyperLinkButtonFocusChange("UserInfoHyperlinkButton");
        }

        private void PublishedWallpaperHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(PublishedWallpaperPage), UserId);
            HyperLinkButtonFocusChange("PublishedWallpaperHyperlinkButton");
        }

        private void FavoriteHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(FavoritePage), UserId);
            HyperLinkButtonFocusChange("FavoriteHyperlinkButton");
        }

        private void FollowHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(FollowPage), UserId);
            HyperLinkButtonFocusChange("FollowHyperlinkButton");
        }

        private void PasswordChangeHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic param = new
            {
                UserInfoDto = UserInfo,
                FromPage = "UserMainPage"
            };
            ContentFrame.Navigate(typeof(PasswordChangePage), param);
            HyperLinkButtonFocusChange("PasswordChangeHyperlinkButton");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PasswordChangeHyperlinkButton.Visibility = Visibility.Collapsed;
            if (e.Parameter != null)
            {
                UserInfo = (UserInfoDto)e.Parameter;
                UserId = UserInfo.U_ID;
                var signinId = SQLiteHelper.GetSigninInfo().SI_UserID;
                if (UserId == signinId)
                {
                    PasswordChangeHyperlinkButton.Visibility = Visibility.Visible;
                }
                ContentFrame.Navigate(typeof(UserPage), e.Parameter);
            }
            else
            {
                ContentFrame.Navigate(typeof(UserPage));
            }
            HyperLinkButtonFocusChange("UserInfoHyperlinkButton");
            base.OnNavigatedTo(e);
        }

        public void HyperLinkButtonFocusChange(string currentFocusName, string content = null)
        {
            HyperlinkButton currentFocus;
            switch (currentFocusName)
            {
                case "FavoriteHyperlinkButton":
                    currentFocus = FavoriteHyperlinkButton;
                    break;
                case "FollowHyperlinkButton":
                    currentFocus = FollowHyperlinkButton;
                    break;
                case "UserInfoHyperlinkButton":
                    currentFocus = UserInfoHyperlinkButton;
                    break;
                case "PasswordChangeHyperlinkButton":
                    currentFocus = PasswordChangeHyperlinkButton;
                    break;
                case "PublishedWallpaperHyperlinkButton":
                    currentFocus = PublishedWallpaperHyperlinkButton;
                    break;
                default:
                    currentFocus = UserInfoHyperlinkButton;
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
    }
}
