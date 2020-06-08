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
    /// 用户主页面
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
            NavigationCacheMode = NavigationCacheMode.Enabled;//启用缓存
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
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        //private void CloseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var rootFrame = Window.Current.Content as Frame;
        //    if (rootFrame.CanGoBack)
        //    {
        //        rootFrame.GoBack();
        //    }
        //}

        /// <summary>
        /// 点击用户信息超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserInfoHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(UserPage), UserInfo);
            HyperLinkButtonFocusChange("UserInfoHyperlinkButton");
        }

        /// <summary>
        /// 点击发布的壁纸超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PublishedWallpaperHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(PublishedWallpaperPage), UserId);
            HyperLinkButtonFocusChange("PublishedWallpaperHyperlinkButton");
        }

        /// <summary>
        /// 点击收藏的壁纸超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FavoriteHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(FavoritePage), UserId);
            HyperLinkButtonFocusChange("FavoriteHyperlinkButton");
        }

        /// <summary>
        /// 点击关注的用户超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FollowHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(FollowPage), UserId);
            HyperLinkButtonFocusChange("FollowHyperlinkButton");
        }

        /// <summary>
        /// 点击修改密码超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordChangeHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic param = new
            {
                UserId = UserInfo.U_ID,
                FromPage = "UserMainPage"
            };
            ContentFrame.Navigate(typeof(PasswordChangePage), param);
            HyperLinkButtonFocusChange("PasswordChangeHyperlinkButton");
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
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
                ContentFrame.Navigate(typeof(UserPage), e.Parameter);//自动导航到用户页面
            }
            else
            {
                ContentFrame.Navigate(typeof(UserPage));
            }
            HyperLinkButtonFocusChange("UserInfoHyperlinkButton");
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
    }
}
