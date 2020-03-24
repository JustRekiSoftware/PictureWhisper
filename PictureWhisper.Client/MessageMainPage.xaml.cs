using PictureWhisper.Client.Helpers;
using PictureWhisper.Client.Views;
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
    public sealed partial class MessageMainPage : Page
    {
        private int UserId { get; set; }
        private HyperlinkButton LastFocus { get; set; }

        public static Frame PageFrame { get; private set; }
        public static MessageMainPage Page { get; private set; }

        public MessageMainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            PageFrame = ContentFrame;
            Page = this;
        }

        private void CommentToUserHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(MessageCommentPage), UserId);
            HyperLinkButtonFocusChange("CommentToUserHyperlinkButton");
        }

        private void ReplyToUserHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(MessageReplyPage), UserId);
            HyperLinkButtonFocusChange("ReplyToUserHyperlinkButton");
        }

        private void ReviewMessageHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(MessageReviewPage), UserId);
            HyperLinkButtonFocusChange("ReviewMessageHyperlinkButton");
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
            if (e.Parameter != null)
            {
                UserId = (int)e.Parameter;
                var signinId = SQLiteHelper.GetSigninInfo().SI_UserID;
                ContentFrame.Navigate(typeof(MessageCommentPage), e.Parameter);
            }
            else
            {
                ContentFrame.Navigate(typeof(MessageCommentPage));
            }
            HyperLinkButtonFocusChange("CommentToUserHyperlinkButton");
            base.OnNavigatedTo(e);
        }

        public void HyperLinkButtonFocusChange(string currentFocusName, string content = null)
        {
            HyperlinkButton currentFocus;
            switch (currentFocusName)
            {
                case "CommentToUserHyperlinkButton":
                    currentFocus = CommentToUserHyperlinkButton;
                    break;
                case "ReplyToUserHyperlinkButton":
                    currentFocus = ReplyToUserHyperlinkButton;
                    break;
                case "ReviewMessageHyperlinkButton":
                    currentFocus = ReviewMessageHyperlinkButton;
                    break;
                default:
                    currentFocus = CommentToUserHyperlinkButton;
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
