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
    /// 动态页面
    /// </summary>
    public sealed partial class SpacePage : Page
    {
        private WallpaperListViewModel WallpaperLVM { get; set; }
        private int CurrentIndex { get; set; }
        private ImageViewModel ImageVM { get; set; }
        private readonly int PageSize = 10;
        private int PageNum { get; set; }
        private int UserId { get; set; }

        public SpacePage()
        {
            WallpaperLVM = new WallpaperListViewModel();
            ImageVM = new ImageViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击上一个或刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex == 0)//刷新
            {
                PageNum = 1;
                await LoadSpaceWallpapersAsync(PageNum++);
            }
            if (CurrentIndex > 0)//上一个
            {
                PrevFontIcon.Glyph = "\xE70E";
                CurrentIndex--;
                ImageVM.Image = WallpaperLVM.SpaceWallpapers[CurrentIndex].Image;
            }
            if (CurrentIndex == 0)//当前壁纸为第一张壁纸，将上一个按钮改为刷新按钮
            {
                PrevFontIcon.Glyph = "\xE72C";
            }
        }

        /// <summary>
        /// 点击下一个按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex < WallpaperLVM.SpaceWallpapers.Count - 1)
            {
                CurrentIndex++;
                ImageVM.Image = WallpaperLVM.SpaceWallpapers[CurrentIndex].Image;
            }
            if (CurrentIndex > WallpaperLVM.SpaceWallpapers.Count - 5)//要到列表尾部时自动加载
            {
                await LoadSpaceWallpapersAsync(PageNum++);
            }
            if (CurrentIndex > 0)
            {
                PrevFontIcon.Glyph = "\xE70E";
            }
        }

        /// <summary>
        /// 点击壁纸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperImageButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(WallpaperMainPage),
                WallpaperLVM.SpaceWallpapers[CurrentIndex].WallpaperInfo);
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (MainPage.Page != null)
            {
                MainPage.Page.HyperLinkButtonFocusChange("SpaceHyperlinkButton");
            }
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
            CurrentIndex = 0;
            await LoadSpaceWallpapersAsync(PageNum);
            if (WallpaperLVM.SpaceWallpapers.Count == 0)
            {
                PrevButton.Visibility = Visibility.Collapsed;
                NextButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ImageVM.Image = WallpaperLVM.SpaceWallpapers[CurrentIndex].Image;
                PrevButton.Visibility = Visibility.Visible;
                NextButton.Visibility = Visibility.Visible;
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 获取动态
        /// </summary>
        /// <param name="page">页数</param>
        /// <returns></returns>
        private async Task LoadSpaceWallpapersAsync(int page)
        {
            if (page == 1)
            {
                await WallpaperLVM.GetSpaceWallpapersAsync(UserId, page, 5);
            }
            else
            {
                await WallpaperLVM.GetSpaceWallpapersAsync(UserId, page, PageSize);
            }
            if (CurrentIndex < WallpaperLVM.SpaceWallpapers.Count)
            {
                ImageVM.Image = WallpaperLVM.SpaceWallpapers[CurrentIndex].Image;
            }
        }
    }
}
