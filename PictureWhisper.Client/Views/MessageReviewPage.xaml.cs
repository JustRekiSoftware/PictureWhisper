using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 审核消息页面
    /// </summary>
    public sealed partial class MessageReviewPage : Page
    {
        private ReviewListViewModel ReviewLVM { get; set; }
        private int UserId { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }

        public MessageReviewPage()
        {
            ReviewLVM = new ReviewListViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// 滑动到底部自动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ReviewScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await ReviewLVM.GetReviewsAsync(UserId, PageNum++, PageSize);
            }
        }

        /// <summary>
        /// 点击刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PageNum = 1;
            await ReviewLVM.GetReviewsAsync(UserId, PageNum++, PageSize);
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NotifyHelper.NotifyTypes.Contains((short)NotifyMessageType.审核))
            {
                NotifyHelper.NotifyTypes.Remove((short)NotifyMessageType.审核);
            }
            if (MessageMainPage.Page != null)
            {
                MessageMainPage.Page.HyperLinkButtonFocusChange("ReviewMessageHyperlinkButton");
            }
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
            await ReviewLVM.GetReviewsAsync(UserId, PageNum++, PageSize);
            base.OnNavigatedTo(e);
        }
    }
}
