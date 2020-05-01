using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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

        private async void WallpaperTypeGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var wallpaperType = (T_WallpaperType)e.ClickedItem;
            WallpaperType = wallpaperType.WT_ID;
            WallpaperTypeGridView.Visibility = Visibility.Collapsed;
            WallpaperScrollViewer.Visibility = Visibility.Visible;
            PageNum = 1;
            await LoadTypeResultAsync(PageNum++);
        }

        private void WallpaperGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var wallpaperDto = (WallpaperDto)e.ClickedItem;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(WallpaperMainPage), wallpaperDto.WallpaperInfo);
        }

        private async void WallpaperScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await LoadTypeResultAsync(PageNum++);
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PageNum = 1;
            await LoadTypeResultAsync(PageNum++);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageNum = 1;
            WallpaperTypeGridView.Visibility = Visibility.Visible;
            WallpaperScrollViewer.Visibility = Visibility.Collapsed;
            await WallpaperTypeLVM.GetWallpaperTypesAsync();
            base.OnNavigatedTo(e);
        }

        private async Task LoadTypeResultAsync(int page)
        {
            await WallpaperLVM.GetTypeResultWallpapersAsync(WallpaperType, page, PageSize);
        }
    }
}
