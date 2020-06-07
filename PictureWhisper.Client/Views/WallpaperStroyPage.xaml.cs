using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 壁纸故事页面
    /// </summary>
    public sealed partial class WallpaperStroyPage : Page
    {
        private WallpaperViewModel WallpaperVM { get; set; }

        public WallpaperStroyPage()
        {
            WallpaperVM = new WallpaperViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;//启用缓存
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (WallpaperMainPage.Page != null)
            {
                WallpaperMainPage.Page.HyperLinkButtonFocusChange("WallpaperStoryHyperlinkButton");
            }
            if (e.Parameter != null)
            {
                var wallpaper = (T_Wallpaper)e.Parameter;
                if (WallpaperVM.Wallpaper.WallpaperInfo == null
                    || WallpaperVM.Wallpaper.WallpaperInfo.W_ID != wallpaper.W_ID)
                {
                    WallpaperVM.Wallpaper.WallpaperInfo = wallpaper;
                }
            }
            base.OnNavigatedTo(e);
        }
    }
}
