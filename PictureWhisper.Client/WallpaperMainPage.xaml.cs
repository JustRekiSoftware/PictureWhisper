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
    /// 壁纸主页面
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

        /// <summary>
        /// 点击壁纸超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperDisplayHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperPage), Wallpaper);
            HyperLinkButtonFocusChange("WallpaperDisplayHyperlinkButton");
        }

        /// <summary>
        /// 点击壁纸故事超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperStoryHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(WallpaperStroyPage), Wallpaper);
            HyperLinkButtonFocusChange("WallpaperStoryHyperlinkButton");
        }

        /// <summary>
        /// 点击评论超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommentHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(CommentPage), Wallpaper);
            HyperLinkButtonFocusChange("CommentHyperlinkButton");
        }

        /// <summary>
        /// 点击回复超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplyHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(ReplyPage));
            HyperLinkButtonFocusChange("ReplyHyperlinkButton");
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var wallpaperInfo = (T_Wallpaper)e.Parameter;
                if (wallpaperInfo.W_ID != Wallpaper.W_ID)
                {
                    Wallpaper = wallpaperInfo;
                    ContentFrame.Navigate(typeof(WallpaperPage), wallpaperInfo);//自动跳转到壁纸页面
                    await SQLiteHelper.AddHistoryInfoAsync(new T_HistoryInfo
                    {
                        HI_WallpaperID = wallpaperInfo.W_ID
                    });//添加浏览记录
                }
                ReplyHyperlinkButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ContentFrame.Navigate(typeof(WallpaperPage));
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
            //显示当前高亮超链接按钮
            if (currentFocus.Visibility == Visibility.Collapsed)
            {
                currentFocus.Visibility = Visibility.Visible;
            }
            //更改超链接按钮内容
            if (content != null && currentFocusName == "ReplyHyperlinkButton")
            {
                ReplyHyperlinkButtonTextBlock.Text = content;
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
