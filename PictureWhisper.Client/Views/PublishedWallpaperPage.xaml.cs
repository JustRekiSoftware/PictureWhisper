using PictureWhisper.Client.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 发布的壁纸页面
    /// </summary>
    public sealed partial class PublishedWallpaperPage : Page
    {
        private WallpaperListViewModel WallpaperLVM { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private int UserId { get; set; }

        public PublishedWallpaperPage()
        {
            WallpaperLVM = new WallpaperListViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        /// <summary>
        /// 点击壁纸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var wallpaperDto = (WallpaperDto)e.ClickedItem;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(WallpaperMainPage), wallpaperDto.WallpaperInfo);
        }

        /// <summary>
        /// 滑动到底部时自动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WallpaperScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await LoadPublishedWallpaperAsync(PageNum++);
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
            await LoadPublishedWallpaperAsync(PageNum++);
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (UserMainPage.Page != null)
            {
                UserMainPage.Page.HyperLinkButtonFocusChange("PublishedWallpaperHyperlinkButton");
            }
            if (e.Parameter != null)
            {
                UserId = (int)e.Parameter;
                PageNum = 1;
                await LoadPublishedWallpaperAsync(PageNum++);
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 加载发布的壁纸
        /// </summary>
        /// <param name="page">页数</param>
        /// <returns></returns>
        private async Task LoadPublishedWallpaperAsync(int page)
        {
            await WallpaperLVM.GetPublishedWallpapersAsync(UserId, page, PageSize);
        }
    }
}
