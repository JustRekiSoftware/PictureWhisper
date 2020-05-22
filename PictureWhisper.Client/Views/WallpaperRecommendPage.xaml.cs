using Microsoft.Toolkit.Uwp.UI.Controls;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 壁纸推荐页面
    /// </summary>
    public sealed partial class WallpaperRecommendPage : Page
    {
        public RecommendWallpaperListViewModel RecommendLVM { get; set; }
        public int UserId { get; set; }
        private readonly int Count = 20;

        public WallpaperRecommendPage()
        {
            RecommendLVM = new RecommendWallpaperListViewModel();
            this.InitializeComponent();
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            NavigationCacheMode = NavigationCacheMode.Enabled;//启用缓存
        }

        /// <summary>
        /// 滑动到底部时自动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RecommendWallpaperScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await LoadRecommendWallpaperAsync();
            }
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
        /// 点击刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RecommendLVM.RecommendWallpapers.Clear();
            await LoadRecommendWallpaperAsync();
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (MainPage.Page != null)
            {
                MainPage.Page.HyperLinkButtonFocusChange("WallpaperRecommendHyperlinkButton");
            }
            if (RecommendLVM != null && RecommendLVM.RecommendWallpapers.Count == 0)
            {
                await LoadRecommendWallpaperAsync();
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 加载推荐壁纸
        /// </summary>
        /// <returns></returns>
        private async Task LoadRecommendWallpaperAsync()
        {
            await RecommendLVM.GetRecommendWallpapersAsync(Count);
        }

        /// <summary>
        /// 窗口改变大小时自动调整瀑布流的列数和大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperStaggeredPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var wallpaperStaggeredPanel = (StaggeredPanel)sender;
            var width = e.NewSize.Width;
            var colSpacing = wallpaperStaggeredPanel.ColumnSpacing;
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
            wallpaperStaggeredPanel.DesiredColumnWidth = (width - (colCount - 1) * colSpacing) / colCount;
        }
    }
}
