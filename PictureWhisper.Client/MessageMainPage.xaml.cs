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
    /// 消息主页面
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
        /// 点击评论消息超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommentToUserHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(MessageCommentPage), UserId);
            HyperLinkButtonFocusChange("CommentToUserHyperlinkButton");
        }

        /// <summary>
        /// 点击回复消息超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplyToUserHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(MessageReplyPage), UserId);
            HyperLinkButtonFocusChange("ReplyToUserHyperlinkButton");
        }

        /// <summary>
        /// 点击审核消息超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReviewMessageHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(MessageReviewPage), UserId);
            HyperLinkButtonFocusChange("ReviewMessageHyperlinkButton");
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //自动导航到评论消息页面
            if (e.Parameter != null)
            {
                ContentFrame.Navigate(typeof(MessageCommentPage), e.Parameter);
            }
            else
            {
                ContentFrame.Navigate(typeof(MessageCommentPage));
            }
            HyperLinkButtonFocusChange("CommentToUserHyperlinkButton");
            //提示新消息
            foreach (var type in NotifyHelper.NotifyTypes)
            {
                switch (type)
                {
                    case (short)NotifyMessageType.评论:
                        CommentToUserHyperlinkButton.Foreground =
                            new SolidColorBrush(ColorHelper.GetLighterAccentColor());
                        break;
                    case (short)NotifyMessageType.回复:
                        ReplyToUserHyperlinkButton.Foreground =
                            new SolidColorBrush(ColorHelper.GetLighterAccentColor());
                        break;
                    case (short)NotifyMessageType.审核:
                        ReviewMessageHyperlinkButton.Foreground =
                            new SolidColorBrush(ColorHelper.GetLighterAccentColor());
                        break;
                    default:
                        break;
                }
            }
            NotifyHelper.NotifyTypes.Clear();
            var settingInfo = SQLiteHelper.GetSettingInfo();
            settingInfo.STI_LastCheckMessageDate = DateTime.Now;
            await SQLiteHelper.UpdateSettingInfoAsync(settingInfo);
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 超链接按钮焦点改变事件
        /// </summary>
        /// <param name="currentFocusName">当前高亮超链接按钮名</param>
        /// <param name="content">当前高亮超链接按钮内容，默认为null</param>
        public void HyperLinkButtonFocusChange(string currentFocusName, string content = null)
        {
            HyperlinkButton currentFocus;
            //获取当前高亮的超链接按钮
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
                LastFocus.Foreground = new SolidColorBrush(ColorHelper.GetForegroudColor());
            }
            //将当前高亮超链接按钮保存为上一次高亮超链接按钮，并更改颜色为高亮色
            LastFocus = currentFocus;
            LastFocus.Foreground = new SolidColorBrush(ColorHelper.GetAccentColor());
        }
    }
}
