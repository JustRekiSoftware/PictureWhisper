using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MUXC = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 按壁纸分区查看壁纸的页面
    /// </summary>
    public sealed partial class WallpaperTypePage : Page
    {
        private WallpaperListViewModel WallpaperLVM { get; set; }
        private WallpaperTypeListViewModel WallpaperTypeLVM { get; set; }
        private short WallpaperType { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }

        public WallpaperTypePage()
        {
            WallpaperLVM = new WallpaperListViewModel();
            WallpaperTypeLVM = new WallpaperTypeListViewModel();
            this.InitializeComponent();
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
                await LoadTypeResultAsync(PageNum++);
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
            await LoadTypeResultAsync(PageNum++);
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (MainPage.Page != null)
            {
                MainPage.Page.HyperLinkButtonFocusChange("WallpaperTypeHyperlinkButton");
            }
            PageNum = 1;
            await WallpaperTypeLVM.GetWallpaperTypesAsync();
            WallpaperType = WallpaperTypeLVM.WallpaperTypes[0].WT_ID;
            TypeNavigationView.SelectedItem = WallpaperTypeLVM.WallpaperTypes[0];
            await LoadTypeResultAsync(PageNum++);
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 加载某分区的壁纸
        /// </summary>
        /// <param name="page">页数</param>
        /// <returns></returns>
        private async Task LoadTypeResultAsync(int page)
        {
            await WallpaperLVM.GetTypeResultWallpapersAsync(WallpaperType, page, PageSize);
        }

        /// <summary>
        /// 切换壁纸分区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void TypeNavigationView_ItemInvoked(MUXC.NavigationView sender, MUXC.NavigationViewItemInvokedEventArgs args)
        {
            var wallpaperType = (T_WallpaperType)args.InvokedItem;
            WallpaperType = wallpaperType.WT_ID;
            PageNum = 1;
            await LoadTypeResultAsync(PageNum++);
        }
    }
}
