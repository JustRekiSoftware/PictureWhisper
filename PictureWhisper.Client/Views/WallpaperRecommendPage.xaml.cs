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
    /// An empty page that can be used on its own or navigated to within a Frame.
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
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            var wallpaper = (WallpaperDto)((Button)sender).DataContext;
            rootFrame.Navigate(typeof(WallpaperMainPage), wallpaper.WallpaperInfo);
        }

        private async void RecommendWallpaperScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await LoadRecommendWallpaperAsync();
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RecommendLVM.RecommendWallpapers.Clear();
            await LoadRecommendWallpaperAsync();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (RecommendLVM != null && RecommendLVM.RecommendWallpapers.Count == 0)
            {
                await LoadRecommendWallpaperAsync();
            }
            base.OnNavigatedTo(e);
        }

        private async Task LoadRecommendWallpaperAsync()
        {
            await RecommendLVM.GetRecommendWallpapersAsync(Count);
        }
    }
}
