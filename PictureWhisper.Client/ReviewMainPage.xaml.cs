using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helpers;
using PictureWhisper.Client.Views;
using PictureWhisper.Domain.Entites;
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

namespace PictureWhisper.Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReviewMainPage : Page
    {
        private T_SigninInfo SigninInfo { get; set; }
        private HyperlinkButton LastFocus { get; set; }

        public static ReviewMainPage Page { get; set; }
        public static Frame PageFrame { get; private set; }

        public ReviewMainPage()
        {
            this.InitializeComponent();
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
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
    }
}
