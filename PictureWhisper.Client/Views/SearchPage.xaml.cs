using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        private bool SearchTypeResult { get; set; }

        public SearchPage()
        {
            this.InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == string.Empty)
            {
                ErrorMessageTextBlock.Text += "错误信息：" + Environment.NewLine;
                ErrorMessageTextBlock.Text += "· 未输入关键词";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;

                return;
            }
            if (SearchTypeResult)
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
