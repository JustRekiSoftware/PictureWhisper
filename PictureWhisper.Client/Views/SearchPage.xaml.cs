using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 搜索页面
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        private bool SearchTypeResult { get; set; }

        public SearchPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击搜索按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == string.Empty)
            {
                ErrorMessageTextBlock.Text += "错误信息：" + Environment.NewLine;
                ErrorMessageTextBlock.Text += "· 未输入关键词";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;

                return;
            }
            if (SearchTypeResult)//跳转到搜索结果页面
            {
                MainPage.PageFrame.Navigate(typeof(UserSearchResultPage), SearchTextBox.Text);
                MainPage.Page.HyperLinkButtonFocusChange(
                    "UserSearchResultHyperlinkButton", SearchTextBox.Text);
            }
            else
            {
                MainPage.PageFrame.Navigate(typeof(WallpaperSearchResultPage), SearchTextBox.Text);
                MainPage.Page.HyperLinkButtonFocusChange(
                    "WallpaperSearchResultHyperlinkButton", SearchTextBox.Text);
            }
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if ((string)e.Parameter == "wallpaper")
            {
                SearchTypeResult = false;
            }
            else
            {
                SearchTypeResult = true;
            }
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            base.OnNavigatedTo(e);
        }
    }
}
