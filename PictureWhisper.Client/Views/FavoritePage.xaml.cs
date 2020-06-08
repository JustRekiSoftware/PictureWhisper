using PictureWhisper.Client.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 收藏页面
    /// </summary>
    public sealed partial class FavoritePage : Page
    {
        private WallpaperListViewModel WallpaperLVM { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private int UserId { get; set; }

        public FavoritePage()
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
        private void WallpaperAdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var wallpaperDto = (WallpaperDto)e.ClickedItem;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(WallpaperMainPage), wallpaperDto.WallpaperInfo);
        }

        /// <summary>
        /// 滑动到底部自动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WallpaperScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await LoadFavoriteWallpaperAsync(PageNum++);
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
            await LoadFavoriteWallpaperAsync(PageNum++);
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (UserMainPage.Page != null)
            {
                UserMainPage.Page.HyperLinkButtonFocusChange("FavoriteHyperlinkButton");
            }
            if (e.Parameter != null)
            {
                UserId = (int)e.Parameter;
                PageNum = 1;
                await LoadFavoriteWallpaperAsync(PageNum++);
            }
            ChangeDesiredWidth(WallpaperAdaptiveGridView.ActualWidth);
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 加载收藏壁纸
        /// </summary>
        /// <param name="page">页数</param>
        /// <returns></returns>
        private async Task LoadFavoriteWallpaperAsync(int page)
        {
            await WallpaperLVM.GetFavoriteWallpapersAsync(UserId, page, PageSize);
        }

        /// <summary>
        /// WallpaperAdaptiveGridView的大小改变时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperAdaptiveGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeDesiredWidth(e.NewSize.Width);
        }

        /// <summary>
        /// 改变WallpaperAdaptiveGridView的物品期望宽度
        /// </summary>
        /// <param name="width">WallpaperAdaptiveGridView的宽度</param>
        public void ChangeDesiredWidth(double width)
        {
            var colCount = 1;
            //计算列数
            if (width >= 1900)
            {
                colCount = 5;
            }
            else if (width >= 1400)
            {
                colCount = 4;
            }
            else if (width >= 1000)
            {
                colCount = 3;
            }
            else if (width >= 600)
            {
                colCount = 2;
            }
            if (WallpaperLVM.FavoriteWallpapers.Count > 0
                && colCount > WallpaperLVM.FavoriteWallpapers.Count)//要显示的物品少于列数
            {
                colCount = WallpaperLVM.FavoriteWallpapers.Count;
            }
            var desiredWidth = width / colCount;
            WallpaperAdaptiveGridView.ItemHeight = desiredWidth * (1080.0 / 1920.0);
            WallpaperAdaptiveGridView.DesiredWidth = desiredWidth;
        }
    }
}
