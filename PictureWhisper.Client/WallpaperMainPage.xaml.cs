using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helper;
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
    public sealed partial class WallpaperMainPage : Page
    {
        private T_Wallpaper Wallpaper { get; set; }
        private HyperlinkButton LastFocus { get; set; }

        public static Frame PageFrame { get; private set; }
        public static WallpaperMainPage Page { get; private set; }

        public WallpaperMainPage()
        {
            Wallpaper = new T_Wallpaper();
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

        private void WallpaperDisplayHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperPage), Wallpaper);
            HyperLinkButtonFocusChange("WallpaperDisplayHyperlinkButton");
        }

        private void WallpaperStoryHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperStroyPage), Wallpaper);
            HyperLinkButtonFocusChange("WallpaperStoryHyperlinkButton");
        }

        private void CommentHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(CommentPage), Wallpaper);
            HyperLinkButtonFocusChange("CommentHyperlinkButton");
        }

        private void ReplyHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(ReplyPage));
            HyperLinkButtonFocusChange("ReplyHyperlinkButton");
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var wallpaperInfo = (T_Wallpaper)e.Parameter;
                if (wallpaperInfo.W_ID != Wallpaper.W_ID)
                {
                    Wallpaper = wallpaperInfo;
                    ContentFrame.Navigate(typeof(WallpaperPage), wallpaperInfo);
                    await SQLiteHelper.AddHistoryInfoAsync(new T_HistoryInfo
                    {
                        HI_WallpaperID = wallpaperInfo.W_ID
                    });
                }
            }
            else
            {
                ContentFrame.Navigate(typeof(WallpaperPage));
            }
            base.OnNavigatedTo(e);
        }

        public void HyperLinkButtonFocusChange(string currentFocusName, string content = null)
        {
            HyperlinkButton currentFocus;
            switch (currentFocusName)
            {
                case "WallpaperDisplayHyperlinkButton":
                    currentFocus = WallpaperDisplayHyperlinkButton;
                    break;
                case "WallpaperStoryHyperlinkButton":
                    currentFocus = WallpaperStoryHyperlinkButton;
                    break;
                case "CommentHyperlinkButton":
                    currentFocus = CommentHyperlinkButton;
                    break;
                case "ReplyHyperlinkButton":
                    currentFocus = ReplyHyperlinkButton;
                    break;
                default:
                    currentFocus = WallpaperDisplayHyperlinkButton;
                    break;
            }
            if (currentFocus.Visibility == Visibility.Collapsed)
            {
                currentFocus.Visibility = Visibility.Visible;
                if (content != null && currentFocusName == "ReplyHyperlinkButton")
                {
                    ReplyHyperlinkButtonTextBlock.Text = content;
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
