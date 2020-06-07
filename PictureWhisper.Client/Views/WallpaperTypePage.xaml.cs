using Microsoft.Toolkit.Uwp.UI.Controls;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System.Drawing;
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
            ChangeDesiredWidth(WallpaperAdaptiveGridView.ActualWidth);
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
            ChangeDesiredWidth(WallpaperAdaptiveGridView.ActualWidth);
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
            if (WallpaperLVM.TypeResultWallpapers.Count > 0
                && colCount > WallpaperLVM.TypeResultWallpapers.Count)//要显示的物品少于列数
            {
                colCount = WallpaperLVM.TypeResultWallpapers.Count;
            }
            var desiredWidth = width / colCount;
            WallpaperAdaptiveGridView.ItemHeight = desiredWidth * (1080.0 / 1920.0);
            WallpaperAdaptiveGridView.DesiredWidth = desiredWidth;
        }
    }
}
